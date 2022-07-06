using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPI.Models;

namespace WeatherAPI.Tests.Services
{
    internal class CityServiceTest
    {
        private Mock<CityContext> _cityContext;
        private Mock<HttpClient> _httpClient;

        [SetUp]
        public void SetUp() { 
            _httpClient = new Mock<HttpClient>();
            _cityContext = new Mock<CityContext>();
        }
       
    }
}
