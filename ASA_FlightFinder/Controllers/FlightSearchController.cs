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
            ViewBag.FromAirport = ViewBag.ToAirport = GetAllAirports();
            return View();
        }
        private class AirportModel
        {
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
        private IEnumerable<SelectListItem> GetAllAirports()
        {
            List<SelectListItem> airports = new List<SelectListItem>();
            airports.Add(new SelectListItem { Text = "Select", Value = "---", Selected = true, Disabled = true });

            using (var reader = System.IO.File.OpenText(Server.MapPath("~/App_Data/airports.csv")))
            {
                using (var csv = new CsvHelper.CsvReader(reader))
                {
                    IEnumerable<AirportModel> file_airports = csv.GetRecords<AirportModel>();

                    foreach (var airport in file_airports)
                    {
                        airports.Add(new SelectListItem { Text = airport.Name, Value = airport.Code });
                    }
                }
            }

            return airports;
        }


        public ActionResult ListFlights(string fromAirport, string toAirport)
        {
            ViewData["FromAirport"] = fromAirport;
            ViewData["ToAirport"] = toAirport;

            List<FlightModel> flights = new List<FlightModel>();

            using (var reader = System.IO.File.OpenText(Server.MapPath("~/App_Data/flights.csv")))
            {
                using (var csv = new CsvHelper.CsvReader(reader))
                {
                    IEnumerable<FlightModel> file_flights = csv.GetRecords<FlightModel>();

                    foreach (var flight in file_flights)
                    {
                        if (flight.From.Equals(fromAirport) && flight.To.Equals(toAirport))
                        {
                            flights.Add(flight);
                        }
                    }

                }
            }

            return View(flights);
        }
    }
}