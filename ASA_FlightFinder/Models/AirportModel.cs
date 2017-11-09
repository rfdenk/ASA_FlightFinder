using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASA_FlightFinder.Models
{
    public class AirportModel
    {
        public interface Visitor
        {
            void AcceptAirport(AirportModel airport);
        }

        public AirportModel()
        {
            this.Code = "";
            this.Name = "";
        }
        public AirportModel(string Code, string Name)
        {
            this.Code = Code;
            this.Name = Name;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}