using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPI.Controllers;
using WeatherAPI.Models;
using WeatherAPI.Services.CityServices;

namespace WeatherAPI.Tests.Controllers;
internal class CityControllerTests
{
    private Mock<ICityService> _cityService;

    CityController _controller;

    [SetUp]
    public void Setup()
    {
        _cityService = new Mock<ICityService>();
        _controller = new CityController(_cityService.Object);
    }

    [Test]
    public async Task GetCityListAsync_Should_Return_Correct_City_List()
    {
        // Arrange
        var expectedCityList = new List<City>()
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

        _cityService.Setup(c => c.GetAllCity())
            .ReturnsAsync(expectedCityList);

        // Act
        var result = await _controller.GetCityList();

        // Assert
        result.Should().BeOfType(typeof(ActionResult<IEnumerable<City>>));
        result.Value.Should().BeEquivalentTo(expectedCityList);
    }

    [Test]
    public async Task GetCityByCityNameAsync_Should_Return_Correct_City()
    {
        // Arrange
        City expectedCity = new City()
        {
            Name = "Berlin",
            Latitude = 52.52,
            Longitude = 13.419998
        };

        _cityService.Setup(c => c.GetCityByCityNameAsync("Berlin"))
            .ReturnsAsync(expectedCity);

        // Act
        var result = await _controller.GetCityByCityName("Berlin");

        // Assert
        result.Should().BeOfType(typeof(ActionResult<City>));
        result.Value.Should().BeEquivalentTo(expectedCity);
    }

    [Test]
    public async Task GetCityByCityNameAsync_With_Invalid_CityName_Should_Return_BadRequest()
    {
        // Arrange
        _cityService.Setup(c => c.GetCityByCityNameAsync("asjdkflsdj"))
            .Throws<HttpRequestException>();

        // Act
        var result = await _controller.GetCityByCityName("asjdkflsdj");

        // Assert
        result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
    }

    [Test]
    public async Task AddCityAsync_Creates_A_City()
    {
        // Arrange
        City newCity = new City()
        {
            Name = "NewCity",
            Latitude = 10,
            Longitude = 20
        };

        _cityService.Setup(c => c.CityExists(newCity.Name))
            .ReturnsAsync(false);

        _cityService.Setup(c => c.AddCityAsync(newCity))
            .ReturnsAsync(newCity);

        // Act
        var actionResult = await _controller.AddCity(newCity);

        // Assert
        var result = actionResult.Result as CreatedAtActionResult;
        result.Value.Should().BeEquivalentTo(newCity);
    }

    [Test]
    public async Task AddCityAsync_With_City_Already_Exist_Should_Conflict()
    {
        // Arrange
        City newCity = new City()
        {
            Name = "NewCity",
            Latitude = 10,
            Longitude = 20
        };

        _cityService.Setup(c => c.CityExists(newCity.Name))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.AddCity(newCity);

        // Assert
        result.Result.Should().BeOfType(typeof(ConflictObjectResult));
    }

    [Test]
    public async Task AddCityAsync_With_City_LatLon_OutofRange_Should_Conflict()
    {
        // Arrange
        City newCity = new City()
        {
            Name = "NewCity",
            Latitude = -180,
            Longitude = 500
        };

        _cityService.Setup(c => c.CityExists(newCity.Name))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.AddCity(newCity);

        // Assert
        result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
    }

    [Test]
    public async Task AddCityAsync_With_City_Lon_OutofRange_Should_Conflict()
    {
        // Arrange
        City newCity = new City()
        {
            Name = "NewCity",
            Latitude = 0,
            Longitude = 1000
        };

        _cityService.Setup(c => c.CityExists(newCity.Name))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.AddCity(newCity);

        // Assert
        result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
    }

    [Test]
    public async Task AddCityAsync_With_City_Lat_OutofRange_Should_Conflict()
    {
        // Arrange
        City newCity = new City()
        {
            Name = "NewCity",
            Latitude = -1800,
            Longitude = 50
        };

        _cityService.Setup(c => c.CityExists(newCity.Name))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.AddCity(newCity);

        // Assert
        result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
    }

    [Test]
    public async Task AddCityAsync_With_City_LatLon_NegOutofRange_Should_Conflict()
    {
        // Arrange
        City newCity = new City()
        {
            Name = "NewCity",
            Latitude = -1800,
            Longitude = -500
        };

        _cityService.Setup(c => c.CityExists(newCity.Name))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.AddCity(newCity);

        // Assert
        result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
    }
    [Test]
    public async Task UpdateCityAsync_Should_Update_City()
    {
        // Arrange
        City cityToUpdate = new City()
        {
            Name = "SomeCity",
            Latitude = 10,
            Longitude = 20
        };

        _cityService.Setup(c => c.CityExists("SomeCity"))
            .ReturnsAsync(true);

        _cityService.Setup(c => c.UpdateCityAsync(cityToUpdate))
            .ReturnsAsync(cityToUpdate);

        // Act
        var result = await _controller.UpdateCity("SomeCity", cityToUpdate);

        // Assert
        result.Should().BeOfType(typeof(ActionResult<City>));
        result.Value.Should().BeEquivalentTo(cityToUpdate);
    }

    [Test]
    public async Task UpdateCityAsync_With_CityName_Not_Matching_City_Name_Should_Return_BadRequest()
    {
        // Arrange
        City cityToUpdate = new City()
        {
            Name = "SomeCity",
            Latitude = 10,
            Longitude = 20
        };

        _cityService.Setup(c => c.CityExists("SomeCity"))
            .ReturnsAsync(true);

        _cityService.Setup(c => c.UpdateCityAsync(cityToUpdate))
            .ReturnsAsync(cityToUpdate);

        // Act
        var result = await _controller.UpdateCity("SomeOtherCity", cityToUpdate);

        // Assert
        result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
    }

    [Test]
    public async Task UpdateCityAsync_With_City_Not_Existing_Should_Return_NotFound()
    {
        // Arrange
        City cityToUpdate = new City()
        {
            Name = "SomeCity",
            Latitude = 10,
            Longitude = 20
        };

        _cityService.Setup(c => c.CityExists("SomeCity"))
            .ReturnsAsync(false);

        _cityService.Setup(c => c.UpdateCityAsync(cityToUpdate))
            .ReturnsAsync(cityToUpdate);

        // Act
        var result = await _controller.UpdateCity("SomeCity", cityToUpdate);

        // Assert
        result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
    }

    [Test]
    public async Task DeleteCityAsync_Should_Delete_City()
    {
        // Arrange
        _cityService.Setup(c => c.CityExists("SomeCity"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCity("SomeCity");

        // Assert
        result.Should().BeOfType(typeof(NoContentResult));
    }

    [Test]
    public async Task DeleteCityAsync_With_NonExistent_CityName_Should_Return_NotFound()
    {
        // Arrange
        _cityService.Setup(c => c.CityExists("SomeCity"))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteCity("SomeCity");

        // Assert
        result.Should().BeOfType(typeof(NotFoundObjectResult));
    }
}
