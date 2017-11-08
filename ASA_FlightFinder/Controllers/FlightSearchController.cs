using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASA_FlightFinder.Models;

namespace ASA_FlightFinder.Controllers
{
    public class FlightSearchController : Controller
    {
        public ActionResult FindFlights()
        {
            ViewData["Message"] = "Flight Finder";
            // These values populate the drop-down lists.
            ViewBag.FromAirport = ViewBag.ToAirport = ListAllAirportSelections();
            return View();
        }




        // Define an airport model, and build a reusable way to visit
        // each airport in the file/datasource.
        private class AirportModel
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

        private void ProcessAllAirports(AirportModel.Visitor visitor)
        {
            using (var reader = System.IO.File.OpenText(Server.MapPath("~/App_Data/airports.csv")))
            {
                using (var csv = new CsvHelper.CsvReader(reader))
                {
                    IEnumerable<AirportModel> file_airports = csv.GetRecords<AirportModel>();
                    foreach (var airport in file_airports)
                    {
                        visitor.AcceptAirport(airport);
                    }
                }
            }
        }

        // This class extracts the airports as a list of airport models.
        private class AirportLister: AirportModel.Visitor
        {
            private List<AirportModel> airports;
            public AirportLister(List<AirportModel> airports)
            {
                this.airports = airports;
            }

            public void AcceptAirport(AirportModel airport)
            {
                airports.Add(airport);
            }
        }

        private class AirportSelectionLister: AirportModel.Visitor
        {
            private List<SelectListItem> items;
            public AirportSelectionLister(List<SelectListItem> items)
            {
                this.items = items;
            }

            public void AcceptAirport(AirportModel airport)
            {
                items.Add(new SelectListItem { Text = airport.Name, Value = airport.Code });
            }
        }

        // Used to populate the dropdown selectors.
        private IEnumerable<SelectListItem> ListAllAirportSelections()
        {
            List<SelectListItem> airports = new List<SelectListItem>();
            airports.Add(new SelectListItem { Text = "Select", Value = "---", Selected = true, Disabled = true });
            ProcessAllAirports(new AirportSelectionLister(airports));
            return airports;
        }

        [Route("FlightSearch/JSON/ListAllAirports")]
        public ActionResult JsonListAllAirports()
        {
            List<AirportModel> airports = new List<AirportModel>();
            ProcessAllAirports(new AirportLister(airports));
            return Json(airports, JsonRequestBehavior.AllowGet);
        }



        // The FlightModel exists elsewhere, along with its Visitor interface.
        // Create a central location to visit each matching flight in the file/datasource.
       
        private void ProcessAllFlights(FlightModel.Visitor visitor)
        {
            using (var reader = System.IO.File.OpenText(Server.MapPath("~/App_Data/flights.csv")))
            {
                using (var csv = new CsvHelper.CsvReader(reader))
                {
                    IEnumerable<FlightModel> file_flights = csv.GetRecords<FlightModel>();

                    foreach (var flight in file_flights)
                    {
                        visitor.AcceptFlight(flight);
                    }
                }
            }
        }

        private class MatchingFlightLister: FlightModel.Visitor
        {
            private string fromAirport;
            private string toAirport;
            private List<FlightModel> flights;
            public MatchingFlightLister(string fromAirport, string toAirport, List<FlightModel> flights)
            {
                this.fromAirport = fromAirport;
                this.toAirport = toAirport;
                this.flights = flights;
            }
            public void AcceptFlight(FlightModel flight)
            {
                // if the caller fails to specify a from, then ignore it.
                // if the caller fails to specify a to, then ignore it.
                // note that if both are unspecified, then this operation will return all flights!
                if (
                    (String.IsNullOrEmpty(fromAirport) || flight.From.Equals(fromAirport)) && 
                    (String.IsNullOrEmpty(toAirport) || flight.To.Equals(toAirport))
                    )
                {
                    flights.Add(flight);
                }
            }
        }

        public ActionResult ListFlights(string fromAirport, string toAirport)
        {
            ViewData["FromAirport"] = fromAirport;
            ViewData["ToAirport"] = toAirport;

            List<FlightModel> flights = new List<FlightModel>();
            ProcessAllFlights(new MatchingFlightLister(fromAirport, toAirport, flights));
            return View(flights);
        }

        [Route("FlightSearch/JSON/ListMatchingFlights")]
        public ActionResult JsonListFlights(string fromAirport, string toAirport)
        {
            List<FlightModel> flights = new List<FlightModel>();
            ProcessAllFlights(new MatchingFlightLister(fromAirport, toAirport, flights));
            return Json(flights, JsonRequestBehavior.AllowGet);
        }
    }
}