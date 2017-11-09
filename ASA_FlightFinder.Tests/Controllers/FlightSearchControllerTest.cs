using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ASA_FlightFinder;
using ASA_FlightFinder.Controllers;

namespace ASA_FlightFinder.Tests.Controllers
{
    [TestClass]
    public class FlightSearchControllerTest
    {
        [TestMethod]
        public void Index()
        {
            FlightSearchController controller = new FlightSearchController();
            ViewResult result = controller.Index() as ViewResult;
            Assert.IsNull(result);  // this is a redirect!
        }

        [TestMethod]
        public void FindFlights()
        {
            FlightSearchController controller = new FlightSearchController();
            ViewResult result = controller.FindFlights(null,null) as ViewResult;
            Assert.IsNotNull(result);

            Assert.IsNotNull(result.ViewBag);
            Assert.IsNotNull(result.ViewBag.FromAirport);
            Assert.AreEqual(result.ViewBag.FromAirport.GetType(), typeof(List<SelectListItem>));
            Assert.AreEqual(result.ViewBag.FromAirport.Count, 5); // four airports and the "select..."

            Assert.IsNotNull(result.ViewBag.ToAirport);
            Assert.AreEqual(result.ViewBag.ToAirport.GetType(), typeof(List<SelectListItem>));
            Assert.AreEqual(result.ViewBag.ToAirport.Count, 5); // four airports and the "select..."

        }
        [TestMethod]
        public void ListFlights()
        {
            FlightSearchController controller = new FlightSearchController();
            ViewResult result = controller.ListFlights("SEA", "LAX") as ViewResult;

            Assert.IsNotNull(result);

            Assert.IsNotNull(result.Model);
            Assert.AreEqual(result.Model.GetType(), typeof(List<Models.FlightModel>));

            var flights = result.Model as List<ASA_FlightFinder.Models.FlightModel>;
            Assert.IsNotNull(flights);

            Assert.AreEqual(flights.Count, 4);

            Assert.AreEqual(result.ViewData["FromAirport"], "SEA");
            Assert.AreEqual(result.ViewData["ToAirport"], "LAX");
            Assert.AreEqual(result.ViewData["FlightCount"], 4);

        }

    }
}
