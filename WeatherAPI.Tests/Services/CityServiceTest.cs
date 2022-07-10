using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPI.Models;
using WeatherAPI.Services;
using WeatherAPI.Services.HttpClients;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.Services.CityServices;

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
                    Name = "Berlin",
                    Latitude = 52.52,
                    Longitude = 13.419998
                },
                new City()
                {
                    Name = "Manchester",
                    Latitude = 30.2,
                    Longitude = 15.419998
                }
            };

            options = new DbContextOptionsBuilder<CityContext>()
                .UseInMemoryDatabase(databaseName: "cityweatherapi")
                .Options;

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
            using (var context = new CityContext(options))
            {
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
            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Arrange
                var newCity = new City()
                {
                    Name = "New City",
                    Latitude = 52.52,
                    Longitude = 13.419998
                };
                var doesCityExistsInDatabase = await cityService.CityExists(newCity.Name);
                doesCityExistsInDatabase.Should().BeFalse();

                // Act
                var result = await cityService.AddCityAsync(newCity);

                // Assert
                result.Should().BeOfType(typeof(City));
                result.Name.Should().Be(newCity.Name);
                result.Latitude.Should().Be(newCity.Latitude);
                result.Longitude.Should().Be(newCity.Longitude);

                var doesCityExists = await cityService.CityExists(newCity.Name);
                doesCityExists.Should().BeTrue();
            }
        }

        [Test]
        public async Task UpdateCityAsync_With_Existing_City_Should_Update()
        {
            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Arrange
                var updatedCity = new City()
                {
                    Name = "Manchester",
                    Latitude = 0.1,
                    Longitude = 0.2
                };

                // Act
                var result = await cityService.UpdateCityAsync(updatedCity);

                // Assert
                result.Should().BeOfType(typeof(City));
                result.Name.Should().Be(updatedCity.Name);
                result.Latitude.Should().Be(updatedCity.Latitude);
                result.Longitude.Should().Be(updatedCity.Longitude);
            }
        }

        [Test]
        public async Task DeleteCityAsync_By_CityName_Should_Delete()
        {
            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

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
                Name = "SomeCity",
                Latitude = 12.34,
                Longitude = 12.42
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
                result.Name.Should().Be(expectedCity.Name);
                result.Latitude.Should().Be(expectedCity.Latitude);
                result.Longitude.Should().Be(expectedCity.Longitude);
            }
        }

        [Test]
        public async Task GetCityByCityNameAsync_With_City_Existing_In_Database_Should_Return_City()
        {
            City expectedCity = new City()
            {
                Name = "Manchester",
                Latitude = 30.2,
                Longitude = 15.419998
            };

            using (var context = new CityContext(options))
            {
                var cityService = new CityService(context, _httpClientService.Object);

                // Act
                var result = await cityService.GetCityByCityNameAsync("Manchester");

                // Assert
                result.Name.Should().Be(expectedCity.Name);
                result.Latitude.Should().Be(expectedCity.Latitude);
                result.Longitude.Should().Be(expectedCity.Longitude);
            }
        }
    }
}
