using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RailcarTripsApp.Server.Controllers;
using RailcarTripsApp.Server.Data;
using RailcarTripsApp.Shared.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore.InMemory;

namespace RailcarTripsApp.Tests
{
    public class TripsControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly TripsController _controller;

        public TripsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new TripsController(_context);
        }

        [Fact]
        public async Task ProcessCsvFile_ValidFile_ReturnsOk()
        {
            // Arrange
            _context.Database.EnsureDeleted(); // Clear the database before each test
            _context.Database.EnsureCreated();

            var cities = new List<City>
            {
                new City { Id = 1, CityName = "City1", TimeZone = "UTC" },
                new City { Id = 2, CityName = "City2", TimeZone = "UTC" }
            };
            _context.Cities.AddRange(cities);
            await _context.SaveChangesAsync();

            var fileMock = new Mock<IFormFile>();
            var content = "Equipment Id,City Id,Event Code,Event Time\nE1,1,W,2023-01-01 12:00:00\nE1,2,Z,2023-01-01 14:00:00";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(content);
            writer.Flush();
            memoryStream.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
            fileMock.Setup(_ => _.FileName).Returns("events.csv");
            fileMock.Setup(_ => _.Length).Returns(memoryStream.Length);

            // Act
            var result = await _controller.ProcessCsvFile(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("File processed successfully, trips generated.", okResult.Value);
        }

        [Fact]
        public async Task GetTrips_ReturnsTrips()
        {
            // Arrange
            _context.Database.EnsureDeleted(); // Clear the database before each test
            _context.Database.EnsureCreated();

            var trips = new List<Trip>
            {
                new Trip { Id = 1, EquipmentId = "E1", OriginCityId = 1, DestinationCityId = 2, StartUTC = DateTime.UtcNow },
                new Trip { Id = 2, EquipmentId = "E2", OriginCityId = 1, DestinationCityId = 2, StartUTC = DateTime.UtcNow.AddHours(1) }
            };
            _context.Trips.AddRange(trips);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetTrips();

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<Trip>>>(result);
            var returnValue = Assert.IsType<List<Trip>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetTripEvents_ValidId_ReturnsEvents()
        {
            // Arrange
            _context.Database.EnsureDeleted(); // Clear the database before each test
            _context.Database.EnsureCreated();

            var trip = new Trip { Id = 1, EquipmentId = "E1", OriginCityId = 1, DestinationCityId = 2, StartUTC = DateTime.UtcNow };
            var events = new List<EquipmentEvent>
            {
                new EquipmentEvent { Id = 1, EquipmentId = "E1", CityId = 1, EventCode = "W", EventDateTimeUTC = DateTime.UtcNow },
                new EquipmentEvent { Id = 2, EquipmentId = "E1", CityId = 2, EventCode = "Z", EventDateTimeUTC = DateTime.UtcNow.AddHours(2) }
            };
            _context.Trips.Add(trip);
            _context.EquipmentEvents.AddRange(events);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetTripEvents(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<EquipmentEvent>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
    }
}




