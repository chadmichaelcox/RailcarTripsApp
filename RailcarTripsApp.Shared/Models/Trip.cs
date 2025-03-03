using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailcarTripsApp.Shared.Models
{
    public class Trip
    {
        [Key]
        public int Id { get; set; }
        public string EquipmentId { get; set; }

        [ForeignKey("OriginCity")]
        public int OriginCityId { get; set; }
        public City OriginCity { get; set; }

        [ForeignKey("DestinationCity")]
        public int DestinationCityId { get; set; }
        public City DestinationCity { get; set; }

        public DateTime StartUTC { get; set; }  
        public DateTime? EndUTC { get; set; }  
        public double? TotalTripHours { get; set; }
    }
}
