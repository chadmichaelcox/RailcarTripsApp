using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailcarTripsApp.Server.Data;
using RailcarTripsApp.Shared.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace RailcarTripsApp.Server.Controllers
{
    [Route("api/trips")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TripsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessCsvFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded");

            using var streamReader = new StreamReader(file.OpenReadStream());
            using var csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture));
            var records = csvReader.GetRecords<EquipmentEventCsv>().ToList();

            var cityMap = _context.Cities.ToDictionary(c => c.Id, c => c.TimeZone);
            var equipmentEvents = new List<EquipmentEvent>();

            foreach (var record in records)
            {
                if (!cityMap.ContainsKey(record.CityId)) continue;

                var localTime = DateTime.Parse(record.EventDateTimeLocal);
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(cityMap[record.CityId]);

                if (timeZone.IsInvalidTime(localTime))
                {
                    Console.WriteLine($"Skipping invalid local time: {localTime} in {timeZone.Id}");
                    continue; 
                }

                var utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime, timeZone);

                equipmentEvents.Add(new EquipmentEvent
                {
                    EquipmentId = record.EquipmentId,
                    CityId = record.CityId,
                    EventCode = record.EventCode,
                    EventDateTimeLocal = localTime,
                    EventDateTimeUTC = utcTime
                });
            }


            _context.EquipmentEvents.AddRange(equipmentEvents);
            await _context.SaveChangesAsync();

            // Process Trips
            var trips = ProcessTrips(equipmentEvents);

            _context.Trips.AddRange(trips);
            await _context.SaveChangesAsync();

            return Ok("File processed successfully, trips generated.");
        }

        private List<Trip> ProcessTrips(List<EquipmentEvent> events)
        {
            var trips = new List<Trip>();
            var groupedEvents = events.OrderBy(e => e.EventDateTimeUTC)
                                      .GroupBy(e => e.EquipmentId)
                                      .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var equipmentId in groupedEvents.Keys)
            {
                var eventsList = groupedEvents[equipmentId];
                Trip? currentTrip = null;

                foreach (var evt in eventsList)
                {
                    if (evt.EventCode == "W")
                    {
                        currentTrip = new Trip
                        {
                            EquipmentId = equipmentId,
                            OriginCityId = evt.CityId,
                            StartUTC = evt.EventDateTimeUTC
                        };
                    }
                    else if (evt.EventCode == "Z" && currentTrip != null)
                    {
                        currentTrip.DestinationCityId = evt.CityId;
                        currentTrip.EndUTC = evt.EventDateTimeUTC;
                        currentTrip.TotalTripHours = (evt.EventDateTimeUTC - currentTrip.StartUTC).TotalHours;

                        trips.Add(currentTrip);
                        currentTrip = null;
                    }
                }
            }
            return trips;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trip>>> GetTrips()
        {
            return await _context.Trips.Include(t => t.OriginCity)
                                       .Include(t => t.DestinationCity)
                                       .OrderBy(t => t.EquipmentId)
                                       .ThenBy(t => t.StartUTC)    
                                       .ToListAsync();
        }

        [HttpGet("{id}/events")]
        public async Task<ActionResult<IEnumerable<EquipmentEvent>>> GetTripEvents(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null) return NotFound();

            var events = await _context.EquipmentEvents
                .Where(e => e.EquipmentId == trip.EquipmentId &&
                            e.EventDateTimeUTC >= trip.StartUTC &&
                            (trip.EndUTC == null || e.EventDateTimeUTC <= trip.EndUTC))
                .OrderBy(e => e.EventDateTimeUTC)
                .Include(e => e.City)
                .ToListAsync();

            return Ok(events);
        }
    }
}
