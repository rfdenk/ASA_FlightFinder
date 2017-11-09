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
                // Note: if this flight is earlier in the day than now,
                // then show it as occurring tomorrow!

                // TODO: Handle timezones!

                // TODO: A real datasource would include the date of the departure in 
                // TODO: record, so this should stop being necessary.

                DateTime time = DateTime.Parse(Departs);    // this inserts today's date automatically.
                DateTime now = DateTime.Now;
                if(time.CompareTo(now) < 0)
                {
                    time = time.AddDays(1.0);
                }
                return time;
            }
        }
        public string Arrives { get; set; }
        public DateTime ArrivalTime {
            get {
                DateTime time = DateTime.Parse(Arrives);

                // Make sure that the flight arrives after it leaves!

                // TODO: A real datasource will include the date in the record, so
                // TODO: this should no longer be necessary.
                // TODO: In that case, validate that the arrival time is after the
                // TODO: departure time!

                while (time.CompareTo(DepartureTime) < 0)
                {
                    time = time.AddDays(1.0);
                }
                return time;
            }
        }
        public decimal MainCabinPrice { get; set; }
        public decimal FirstClassPrice { get; set; }

        public TimeSpan Duration
        {
            get
            {
                return ArrivalTime - DepartureTime;
            }
        }
    }
}