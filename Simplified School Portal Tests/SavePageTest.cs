using System;
using System.Web.Mvc;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simplified_School_Portal.Controllers;
using Simplified_School_Portal.Models;
using Simplified_School_Portal.DAL;
using System.Data.SqlClient;

namespace Simplified_School_Portal_Tests
{
    [TestClass]
    public class SavePageTest
    {
        private SSPDatabaseEntities context = new SSPDatabaseEntities();

        [TestMethod]
        public void addGetCall()
        {
            // Arrange
            bool callAdded = true;
            string exceptionMessage = "";

            Package_callRepository repo = new Package_callRepository(context);

            Package_call call = new Package_call();
            call.Call = "Google Calendar GET";
            call.Call_url = "https://www.google.nl";
            call.Call_verificationNeeded = "No";
            call.Call_type = "GET";

            // Act
            try
            {
                context.Database.Connection.Open();
                repo.InsertPackage_call(call);
                repo.Save();
                context.Database.Connection.Close();
            }
            catch (Exception e)
            {
                callAdded = false;
                exceptionMessage = e.Message;
            }

            // Assert
            Assert.IsTrue(callAdded, exceptionMessage);
        }

        [TestMethod]
        public void addApiPackage()
        {
            // Arrange
            bool packageAdded = true;
            string exceptionMessage = "";

            API_packageRepository repo = new API_packageRepository(context);

            API_package package = new API_package();
            package.Package_name = "Google Calls";
            package.Package_description = "Test description";
            package.Package_url = "https://www.google.nl";
            package.Package_call.Add(new Package_call()
            {
                Call = "Google Calendar GET",
                Call_url = "https://www.google.nl",
                Call_verificationNeeded = "No",
                Call_type = "GET"
            });

            // Act
            try
            {
                repo.InsertApi_package(package);
                repo.Save();
            }
            catch (Exception e)
            {
                packageAdded = false;
                exceptionMessage = e.Message;
            }

            // Assert
            Assert.IsTrue(packageAdded, exceptionMessage);
        }

        [TestMethod]
        public void correctSavePageOutput()
        {
            //Arrange
            BeheerServicesController controller = new BeheerServicesController();
            string exceptionMessage = "";
            string expectedOutput = "<div class=\"row\"><div class=\"col-md-5 dynamicBlock\"><p></p></div><div class=\"col-md-5 col-md-offset-2 dynamicBlock\"><p>Callresults1</p></div></div>";
            string realOutput = "";
            List<Position> positions = new List<Position>();

            Position position = new Position();
            position.y = "0";
            position.x = "6";
            position.w = "3";
            position.h = "3";
            position.content = "Callresults1";


            // Act
            try
            {
                positions.Add(position);
                realOutput = controller.extractCorrectHtmlOutput(positions);
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }


            // Assert
            Assert.AreEqual(expectedOutput, realOutput, exceptionMessage);
        }

        [TestMethod]
        public void correctSavePageOutput2()
        {
            // Arrange
            BeheerServicesController controller = new BeheerServicesController();
            string exceptionMessage = "";
            string expectedOutput = "<div class=\"row\"><div class=\"col-md-5 dynamicBlock\"><p></p></div><div class=\"col-md-5 col-md-offset-2 dynamicBlock\"><p></p></div></div><div class=\"row\"><div class=\"col-md-5 dynamicBlock\"><p>Callresults2</p></div><div class=\"col-md-5 col-md-offset-2 dynamicBlock\"><p></p></div></div>";
            string realOutput = "";
            List<Position> positions = new List<Position>();

            Position position = new Position();
            position.y = "2";
            position.x = "0";
            position.w = "3";
            position.h = "3";
            position.content = "Callresults2";


            // Act
            try
            {
                positions.Add(position);
                realOutput = controller.extractCorrectHtmlOutput(positions);
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }


            // Assert
            Assert.AreEqual(expectedOutput, realOutput, exceptionMessage);
        }
    }
}
