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
        // This is the starting point. It immediately redirects to FindFlights,
        // so that the URL is a bit more comprehensible if we arrive at FindFlights
        // with some URL parameters.
        public ActionResult Index()
        {
            return RedirectToAction("FindFlights", "FlightSearch");
        }

        // Main flight-finder page: allow the user to select flight parameters
        // and then bounce to ListFlights.
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




        // A reusable way to visit each airport in the file/datasource.
        private void ProcessAllAirports(AirportModel.Visitor visitor)
        {
            try
            {
                using (var reader = System.IO.File.OpenText(FileFinder.DatasourcePath(this, "airports.csv")))
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
            catch(Exception e)
            {
                // Don't send any (more) entries to the visitor.
            }
        }



        // Create a list of SelectListItems for use in <select>.
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
                // If this airport code matches the selected airport code, then select it
                // in the options list. This will override the default selection of "Select..."
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
        private IEnumerable<SelectListItem> ListAllAirportSelections(string selected)
        {
            List<SelectListItem> airports = new List<SelectListItem>();
            ProcessAllAirports(new AirportSelectionLister(airports, selected));

            // If any element is already selected, 
            // then don't select the "Select..." option;
            // otherwise, do.
            bool alreadySelected = (airports.Find(delegate (SelectListItem item) { return item.Selected; })!=null);

            SelectListItem dummyEntry = new SelectListItem { Text = "Select...", Value = "---", Selected = !alreadySelected, Disabled = true };
            airports.Insert(0, dummyEntry);

            return airports;
        }

        // This class extracts the airports as a list of airport models.
        private class AirportLister : AirportModel.Visitor
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

        [Route("FlightSearch/JSON/ListAllAirports")]
        public ActionResult JsonListAllAirports()
        {
            List<AirportModel> airports = new List<AirportModel>();
            ProcessAllAirports(new AirportLister(airports));
            return Json(airports, JsonRequestBehavior.AllowGet);
        }



        // Create a central location to visit each flight in the file/datasource.
        private void ProcessAllFlights(FlightModel.Visitor visitor)
        {
            try
            {
                using (var reader = System.IO.File.OpenText(FileFinder.DatasourcePath(this, "flights.csv")))
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
            catch (Exception e){
                // Don't send any (more) entries to the visitor.
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
            ProcessAllFlights(new MatchingFlightLister(fromAirport, toAirport, flights, allowMissing: false));

            ViewData["FlightCount"] = flights.Count;

            return View(flights);
        }

        [Route("FlightSearch/JSON/ListMatchingFlights")]
        public ActionResult JsonListFlights(string fromAirport, string toAirport)
        {
            List<FlightModel> flights = new List<FlightModel>();
            ProcessAllFlights(new MatchingFlightLister(fromAirport, toAirport, flights));
            return Json(flights, JsonRequestBehavior.AllowGet);
        }




        // Get the data from a different place for testing!
        private class FileFinder
        {
            public static string DatasourcePath(FlightSearchController controller, string filename)
            {
                if (controller.Server == null)
                {
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\" + filename);
                    return path;
                }
                return controller.Server.MapPath("~/App_Data/" + filename);
            }
        }   // FileFinder

    }   // class
}   // namespace