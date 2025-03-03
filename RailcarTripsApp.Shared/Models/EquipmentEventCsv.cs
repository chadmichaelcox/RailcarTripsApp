using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailcarTripsApp.Shared.Models
{
    public class EquipmentEventCsv
    {
        [Name("Equipment Id")] 
        public string EquipmentId { get; set; }

        [Name("City Id")] 
        public int CityId { get; set; }

        [Name("Event Code")] 
        public string EventCode { get; set; }

        [Name("Event Time")] 
        public string EventDateTimeLocal { get; set; }
    }

}
