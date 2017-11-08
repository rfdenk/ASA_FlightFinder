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
            // Arrange
            FlightSearchController controller = new FlightSearchController();

            // Act
            ViewResult result = controller.FindFlights() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ListFlights()
        {
            // Arrange
            FlightSearchController controller = new FlightSearchController();

            // Act
            ViewResult result = controller.ListFlights("SEA","LAX") as ViewResult;

            // Assert
            //Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }
        
    }
}
