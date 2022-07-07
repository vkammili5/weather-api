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
        private Mock<IHttpClientService> _httpClientService;
        private DbContextOptions<CityContext> options;
        private List<City> defaultCities;

        [SetUp]
        public void SetUp() {
            // Arrange
            _httpClientService = new Mock<IHttpClientService>();

            defaultCities = new List<City>()
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

            options = new DbContextOptionsBuilder<CityContext>()
                .UseInMemoryDatabase(databaseName: "cityweatherapi")
                .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new CityContext(options))
            {
                foreach (var city in context.Cities)
                    context.Cities.Remove(city);

                foreach (var city in defaultCities)
                    context.Cities.Add(city);

                context.SaveChanges();
            }
        }

        [Test]
        public async Task GetAllCity_Should_Return_AllCity()
        {
            // Use a clean instance of the context to run the test
            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Act
                var result = await cityService.GetAllCity();

                // Assert
                result.Should().BeEquivalentTo(defaultCities);
            }
        }

        [Test]
        public async Task GetAllCity_With_No_City_Should_Return_Empty()
        {
            // Use a clean instance of the context to run the test
            using (var context = new CityContext(options))
            {
                // remove all cities in database
                foreach (var city in context.Cities)
                    context.Cities.Remove(city);

                context.SaveChanges();

                var cityService = new CityService(context, _httpClientService.Object);

                // Act
                var result = await cityService.GetAllCity();

                // Assert
                result.Count.Should().Be(0);
            }
        }

        [Test]
        public async Task CityExists_With_Existing_City_Should_Return_True()
        {
            // Use a clean instance of the context to run the test
            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Act
                var result = await cityService.CityExists("Berlin");

                // Assert
                result.Should().BeTrue();
            }
        }

        [Test]
        public async Task CityExists_With_Non_Existing_City_Should_Return_False()
        {
            // Use a clean instance of the context to run the test
            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Act
                var result = await cityService.CityExists("Random City");

                // Assert
                result.Should().BeFalse();
            }
        }

        [Test]
        public async Task CityExists_With_Existing_City_Wrong_Case_Should_Return_True()
        {
            // Use a clean instance of the context to run the test
            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Act
                var result = await cityService.CityExists("BeRlIn");

                // Assert
                result.Should().BeTrue();
            }
        }

        [Test]
        public async Task AddCityAsync_City_shouldnot_Exist()
        {
            // Use a clean instance of the context to run the test
            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Arrange
                var newCity = new City()
                {
                    name = "New City",
                    latitude = 52.52,
                    longitude = 13.419998
                };

                // Act
                var result = await cityService.AddCityAsync(newCity);

                // Assert
                result.Should().BeOfType(typeof(City));
                result.name.Should().Be(newCity.name);
                result.latitude.Should().Be(newCity.latitude);
                result.longitude.Should().Be(newCity.longitude);

                var doesCityExists = await cityService.CityExists(newCity.name);
                doesCityExists.Should().BeTrue();
            }
        }

        [Test]
        public async Task UpdateCityAsync_With_Existing_City_Should_Update()
        {
            // Use a clean instance of the context to run the test
            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Arrange
                var updatedCity = new City()
                {
                    name = "Manchester",
                    latitude = 0.1,
                    longitude = 0.2
                };

                // Act
                var result = await cityService.UpdateCityAsync(updatedCity);

                // Assert
                result.Should().BeOfType(typeof(City));
                result.name.Should().Be(updatedCity.name);
                result.latitude.Should().Be(updatedCity.latitude);
                result.longitude.Should().Be(updatedCity.longitude);
            }
        }

        [Test]
        public async Task DeleteCityAsync_By_CityName_Should_Delete()
        {
            // Use a clean instance of the context to run the test
            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Arrange

                // Act
                await cityService.DeleteCityAsync("Berlin");
                var result = await cityService.CityExists("Berlin");

                // Assert
                result.Should().BeFalse();
            }
        }

        [Test]
        public async Task GetCityByCityNameAsync_With_City_NotMatching_Database_City_Should_Return_City()
        {
            // Arrange
            var expectedCity = new City()
            {
                name = "SomeCity",
                latitude = 12.34,
                longitude = 12.42
            };

            string responseString = "{\"results\":" +
                "[{ " +
                "\"id\":2643123," +
                "\"name\":\"SomeCity\"," +
                "\"latitude\":12.34," +
                "\"longitude\":12.42}], " +
                "\"generationtime_ms\":0.48303604}";

            string url = $"https://geocoding-api.open-meteo.com/v1/search?name=SomeCity";

            _httpClientService.Setup(x => x.GetAsync(url))
                .ReturnsAsync((responseString, true));

            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Act
                var result = await cityService.GetCityByCityNameAsync("SomeCity");

                // Assert
                result.name.Should().Be(expectedCity.name);
                result.latitude.Should().Be(expectedCity.latitude);
                result.longitude.Should().Be(expectedCity.longitude);
            }
        }

        [Test]
        public async Task GetCityByCityNameAsync_With_City_Existing_In_Database_Should_Return_City()
        {
            City expectedCity = new City()
            {
                name = "Manchester",
                latitude = 30.2,
                longitude = 15.419998
            };

            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Act
                var result = await cityService.GetCityByCityNameAsync("Manchester");

                // Assert
                result.name.Should().Be(expectedCity.name);
                result.latitude.Should().Be(expectedCity.latitude);
                result.longitude.Should().Be(expectedCity.longitude);
            }
        }
    }
}
