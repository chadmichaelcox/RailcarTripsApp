using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailcarTripsApp.Shared.Models
{
    public class EquipmentEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EquipmentId { get; set; } = string.Empty;

        [Required]
        public int CityId { get; set; }

        public City City { get; set; } = new City();

        [Required]
        [ForeignKey("EventCode")]
        public string EventCode { get; set; } = string.Empty;

        [Required]
        public DateTime EventDateTimeLocal { get; set; }

        [Required]
        public DateTime EventDateTimeUTC { get; set; }
    }
}
