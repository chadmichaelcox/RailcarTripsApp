using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace RailcarTripsApp.Shared.Models
{
    public class City
    {
        [Key]
        [Ignore]
        public int Id { get; set; }

        [Required]
        [Name("City Name")]
        public string CityName { get; set; } = string.Empty;

        [Required]
        [Name("Time Zone")]
        public string TimeZone { get; set; } = string.Empty;
    }
}
