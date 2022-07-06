using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPI.Models;
using WeatherAPI.Services;
using WeatherAPI.Services.HttpClients;
using Microsoft.EntityFrameworkCore;

namespace WeatherAPI.Tests.Services
{
    internal class CityServiceTest
    {
        private CityService _service;
        private Mock<IHttpClientService> _httpClientService;

        [SetUp]
        public void SetUp() {
            _httpClientService = new Mock<IHttpClientService>();
        }

        [Test]
        public async Task GetAllCity_Should_Return_AllCity()
        {
            // Arrange
            var expectedCities = new List<City>()
            {
                new City()
                {
                    name = "Berlin",
                    latitude = 52.52,
                    longitude = 13.419998
                },
                new City()
                {
                    name = "Manchester",
                    latitude = 30.2,
                    longitude = 15.419998
                }
            };

            var options = new DbContextOptionsBuilder<CityContext>()
                .UseInMemoryDatabase(databaseName: "cityweatherapi")
                .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new CityContext(options))
            {
                foreach (var city in expectedCities)
                    context.Cities.Add(city);

                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new CityContext(options))
            {
                _service = new CityService(context, _httpClientService.Object);

                // Act
                var result = await _service.GetAllCity();

                // Assert
                result.Should().BeEquivalentTo(expectedCities);
            }
        }
    }
}
