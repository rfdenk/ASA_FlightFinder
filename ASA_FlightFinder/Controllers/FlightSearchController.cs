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
        public ActionResult Index()
        {
            return RedirectToAction("FindFlights", "FlightSearch");
        }
        public ActionResult FindFlights(string from, string to)
        {
            ViewData["Message"] = "Flight Finder";

            // These values populate the drop-down lists.
            // We _can_ wind up here with a from and/or a to, if the user purposely
            // manipulates the URL. If so, go ahead and "pre-select" the correct airport.
            ViewBag.FromAirport = ListAllAirportSelections(from);
            ViewBag.ToAirport = ListAllAirportSelections(to);
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
            string selected;
            public AirportSelectionLister(List<SelectListItem> items, string selected)
            {
                this.items = items;
                this.selected = selected;
            }

            public void AcceptAirport(AirportModel airport)
            {
                if (!String.IsNullOrEmpty(this.selected) && airport.Code.Equals(this.selected))
                {
                    items.Add(new SelectListItem { Text = airport.Name, Value = airport.Code, Selected = true });
                }
                else
                {
                    items.Add(new SelectListItem { Text = airport.Name, Value = airport.Code });
                }
            }
        }

        // Used to populate the dropdown selectors.
        private IEnumerable<SelectListItem> ListAllAirportSelections(string select)
        {
            List<SelectListItem> airports = new List<SelectListItem>();
            airports.Add(new SelectListItem { Text = "Select", Value = "---", Selected = true, Disabled = true });
            ProcessAllAirports(new AirportSelectionLister(airports, select));
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
            private bool allowMissing;

            public MatchingFlightLister(
                string fromAirport, string toAirport, 
                List<FlightModel> flights, bool allowMissing = true)
            {
                this.fromAirport = fromAirport;
                this.toAirport = toAirport;
                this.flights = flights;
                this.allowMissing = allowMissing;
            }
            private bool matchElement(string myString, string flightString)
            {
                // if the caller allowsMissing, then
                //    if the caller fails to specify a from, then ignore it.
                //    if the caller fails to specify a to, then ignore it.
                //    note that if both are unspecified, then this operation will return all flights!
                if (this.allowMissing && String.IsNullOrEmpty(myString)) return true;
                if (String.IsNullOrEmpty(myString)) return false;
                return myString.Equals(flightString);
            }
            public void AcceptFlight(FlightModel flight)
            {
                if (matchElement(this.fromAirport, flight.From) && matchElement(this.toAirport, flight.To))
                {
                    flights.Add(flight);
                }
            }
        }

        public ActionResult ListFlights(string fromAirport, string toAirport)
        {
            // Gracefully fail if fromAirport or toAirport is missing.
            // We bounce back to the FlightFinder, and preselect the airports that
            // are not missing.
            if (String.IsNullOrEmpty(fromAirport))
            {
                return RedirectToAction("FindFlights", "FlightSearch", new { To = toAirport });
            }
            if (String.IsNullOrEmpty(toAirport))
            {
                return RedirectToAction("FindFlights", "FlightSearch", new { From = fromAirport });
            }

            ViewData["FromAirport"] = fromAirport;
            ViewData["ToAirport"] = toAirport;

            List<FlightModel> flights = new List<FlightModel>();
            ProcessAllFlights(new MatchingFlightLister(fromAirport, toAirport, flights, false));
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