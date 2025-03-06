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
    public class DataSeederControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly DataSeederController _controller;

        public DataSeederControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new DataSeederController(_context);
        }

        [Fact]
        public async Task SeedCities_ValidFile_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Id,City Name,Time Zone\n1,City1,TimeZone1\n2,City2,TimeZone2";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(content);
            writer.Flush();
            memoryStream.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
            fileMock.Setup(_ => _.FileName).Returns("cities.csv");
            fileMock.Setup(_ => _.Length).Returns(memoryStream.Length);

            // Act
            var result = await _controller.SeedCities(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cities data seeded successfully.", okResult.Value);
        }

        [Fact]
        public async Task SeedEventCodes_ValidFile_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Event Code,Event Description,Long Description\nE1,Description1,LongDescription1\nE2,Description2,LongDescription2";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(content);
            writer.Flush();
            memoryStream.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
            fileMock.Setup(_ => _.FileName).Returns("eventcodes.csv");
            fileMock.Setup(_ => _.Length).Returns(memoryStream.Length);

            // Act
            var result = await _controller.SeedEventCodes(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Event codes data seeded successfully.", okResult.Value);
        }
    }


}
