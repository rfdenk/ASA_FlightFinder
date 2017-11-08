using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// This model is used to read data from the Flights.CSV file,
// and to populate the webpage's flights list.

namespace ASA_FlightFinder.Models
{
    public class FlightModel
    {
        public interface Visitor
        {
            void AcceptFlight(FlightModel flight);
        }
        public FlightModel()
        {
            this.From = "";
            this.To = "";
            this.FlightNumber = "";
            this.Departs = "0001-01-01";
            this.Arrives = "0001-01-01";
            this.MainCabinPrice = 0.0M;
            this.FirstClassPrice = 0.0M;
        }
        public FlightModel(string From, string To, string FlightNumber, 
            string Departs, string Arrives, 
            decimal MainCabinPrice, decimal FirstClassPrice
            )
        {
            this.From = From;
            this.To = To;
            this.FlightNumber = FlightNumber;
            this.Departs = Departs;
            this.Arrives = Arrives;
            this.MainCabinPrice = MainCabinPrice;
            this.FirstClassPrice = FirstClassPrice;
        }
        public string From { get; set; }
        public string To { get; set; }
        public string FlightNumber { get; set; }
        public string Departs { set; get; }
        public DateTime DepartureTime {
            get {
                // Naughty: This inserts the current date, which is weird.
                DateTime time = DateTime.Parse(Departs);
                return time;
            }
        }
        public string Arrives { get; set; }
        public DateTime ArrivalTime {
            get {
                // Naughty: This inserts the current date, which is weird.
                return DateTime.Parse(Arrives);
            }
        }
        public decimal MainCabinPrice { get; set; }
        public decimal FirstClassPrice { get; set; }

    }
}