using CsvHelper.Configuration.Attributes;

namespace RailcarTripsApp.Shared.Models
{
    public class EquipmentEventCsv
    {
        [Name("Equipment Id")]
        public string EquipmentId { get; set; } = string.Empty;

        [Name("City Id")]
        public int CityId { get; set; }

        [Name("Event Code")]
        public string EventCode { get; set; } = string.Empty;

        [Name("Event Time")]
        public string EventDateTimeLocal { get; set; } = string.Empty;
    }
}
