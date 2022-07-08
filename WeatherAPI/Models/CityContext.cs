
using Microsoft.EntityFrameworkCore;

namespace WeatherAPI.Models
{
    public class CityContext : DbContext
    {
        public CityContext(DbContextOptions<CityContext> options) : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
    }
}

