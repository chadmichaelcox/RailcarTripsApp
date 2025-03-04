using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RailcarTripsApp.Shared.Models
{
    public class EventCodeDefinition
    {
        [Key]
        [StringLength(10)]
        [Name("Event Code")]
        public string EventCode { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Name("Event Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Name("Long Description")]
        public string LongDescription { get; set; } = string.Empty;
    }
}
