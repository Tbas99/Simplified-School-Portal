using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simplified_School_Portal.Controllers;
using Simplified_School_Portal.Models;
using Simplified_School_Portal.DAL;

namespace Simplified_School_Portal_Tests
{
    [TestClass]
    public class CommunicationComponentTest
    {
        private SSPDatabaseEntities context = new SSPDatabaseEntities();

        [TestMethod]
        public void sendInfoRequest()
        {
            // Arrange
            bool mailSend = false;
            string exceptionMessage = "";
            HomeController controller = new HomeController();
            Mailmodel sendMail = new Mailmodel()
            {
                Name = "Tobias",
                Email = "Tobiassagis@gmail.com",
                Description = "This is my problem"
            };
            string fromAddress = "tobiassagis@gmail.com";
            string toAddress = "tobiassagis@gmail.com";
            string subject = "Question from " + sendMail.Name;


            // Act
            if (sendMail.IsValidEmail())
            {
                try
                {
                    controller.SendEmail(toAddress, fromAddress, subject, sendMail.Description);
                    mailSend = true;
                }
                catch (Exception e)
                {
                    exceptionMessage = e.Message;
                }
            }

            // Assert
            Assert.IsTrue(mailSend, exceptionMessage);

        }

        [TestMethod]
        public void addFeatureRequest()
        {
            // Arrange
            API_packageRepository repoApiPackage = new API_packageRepository(context);
            Feature_requestRepository repoFeatureReq = new Feature_requestRepository(context);
            List<Feature_request> rows = (List<Feature_request>)repoFeatureReq.GetFeatureRequests();
            int originalCount = rows.Count;

            Feature_request request = new Feature_request();
            request.Feature_request_issuer = "Tobias";
            request.Feature_name = "Google Calendar";
            request.Feature_develop_url = "http://www.google.nl";

            string isImplemented = "false";
            foreach (API_package package in repoApiPackage.GetPackages())
            {
                if (package.Package_name == request.Feature_name)
                {
                    isImplemented = "true";
                }
            }

            request.Feature_is_implemented = isImplemented;
            request.Feature_request_date = DateTime.Now;


            // Act
            repoFeatureReq.InsertFeature_request(request);
            repoFeatureReq.Save();


            // Assert
            List<Feature_request> newRowCount = (List<Feature_request>)repoFeatureReq.GetFeatureRequests();
            Assert.AreEqual(originalCount + 1, newRowCount.Count);
        }
    }
}
