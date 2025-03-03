using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailcarTripsApp.Server.Data;
using RailcarTripsApp.Shared.Models;
using CsvHelper;
using System.Globalization;

namespace RailcarTripsApp.Server.Controllers
{
    [Route("api/data-seed")]
    [ApiController]
    public class DataSeederController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DataSeederController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("seed-cities")]
        public async Task<IActionResult> SeedCities([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<City>().ToList();

            _context.Cities.AddRange(records);
            await _context.SaveChangesAsync();

            return Ok("Cities data seeded successfully.");
        }

        [HttpPost("seed-event-codes")]
        public async Task<IActionResult> SeedEventCodes([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<EventCodeDefinition>().ToList();

            _context.EventCodes.AddRange(records);
            await _context.SaveChangesAsync();

            return Ok("Event codes data seeded successfully.");
        }
    }
}
