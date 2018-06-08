using System;
using System.Web;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simplified_School_Portal.Controllers;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Simplified_School_Portal_Tests
{
    [TestClass]
    public class APITest
    {
        [TestMethod]
        public void LoginTest()
        {
            // Arrange
            bool connectionWorks = false;
            StandardServicesController controller = new StandardServicesController();
            string host = "https://identity.fhict.nl/connect/authorize?client_id=i387766-simplified&scope=fhict fhict_personal openid profile email roles&response_type=code&redirect_uri=https://localhost:44363/StandardServices/Callback";
            HttpClient client = new HttpClient();

            // Act
            var response = client.GetAsync(host);
            if (response != null)
            {
                connectionWorks = true;
            }

            // Assert
            Assert.IsTrue(connectionWorks);
        }

        [TestMethod]
        public async Task getLesrooster()
        {
            // Arrange
            bool connectionWorks = false;
            StandardServicesController controller = new StandardServicesController();
            string callAddress = "https://api.fhict.nl/schedule/me";
            string temporaryToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IlZickNEN1NyWFhSVDYzYVVpbktPZm11cl9xcyIsImtpZCI6IlZickNEN1NyWFhSVDYzYVVpbktPZm11cl9xcyJ9.eyJpc3MiOiJodHRwczovL2lkZW50aXR5LmZoaWN0Lm5sIiwiYXVkIjoiaHR0cHM6Ly9pZGVudGl0eS5maGljdC5ubC9yZXNvdXJjZXMiLCJleHAiOjE1Mjg0NjQ2NDUsIm5iZiI6MTUyODQ1NzQ0NSwiY2xpZW50X2lkIjoiYXBpLWNsaWVudCIsInVybjpubC5maGljdDp0cnVzdGVkX2NsaWVudCI6InRydWUiLCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwiZW1haWwiLCJmaGljdCIsImZoaWN0X3BlcnNvbmFsIiwiZmhpY3RfbG9jYXRpb24iXSwic3ViIjoiYWFhYTQ3ZGEtZGU2OS00YmMwLTk0NjktMDA4NDU2MTI4YTZiIiwiYXV0aF90aW1lIjoxNTI4NDU3NDQ0LCJpZHAiOiJmaGljdC1zc28iLCJyb2xlIjpbInVzZXIiLCJzdHVkZW50Il0sInVwbiI6IkkzODc3NjZAZmhpY3QubmwiLCJuYW1lIjoiU2FnaXMsVG9iaWFzIFQuRy5NLiIsImVtYWlsIjoidC5zYWdpc0BzdHVkZW50LmZvbnR5cy5ubCIsInVybjpubC5maGljdDpzY2hlZHVsZSI6ImNsYXNzfFMyMiIsImZvbnR5c191cG4iOiIzODc3NjZAc3R1ZGVudC5mb250eXMubmwiLCJhbXIiOlsiZXh0ZXJuYWwiXX0.L6zu4zBzIUzF0bzIP7xshUul-b7kYX6elq1Lhw6vUO61LjgE7kLpdpicEJAT_HprDcezCwfpJuZ8WbRmYHlPLprdcTwR1iijQSCBpj41z8iaatWhZiZ310gc1xT2ZEjxeBCXxVaLmkpS5mw_rXzB9dmMs5lZd7HzkCFg1klZwvlgLdK18LqIvv58zZ_S8OZQz8LrlQxE-7vIdNdYMXVXMZZjPBAdSoLkNTeXU7d7UOT6JuWH596I-KCX2lm50vJAQWhUhH2Ed0QU-lU90aHZYyxU4bqKCmIgeojaUwHoV5J9a1XONTTehaVU2BprT1OR7aYrwuVkx-QhqH88RSa_Mw";
            HttpClient client = new HttpClient();

            // Act
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", temporaryToken);
            var response = await client.GetAsync(callAddress);
            if (response.ReasonPhrase == "OK")
            {
                connectionWorks = true;
            }

            // Assert
            Assert.IsTrue(connectionWorks);
        }
    }
}
