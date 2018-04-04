using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using Simplified_School_Portal.Models;
using System.Threading;
using System.Text;

namespace Simplified_School_Portal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }


        // Code that handles the contact form emailing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(Mailmodel m)
        {
            if (ModelState.IsValid)
            {
                var fromAddress = "tobiassagis@gmail.com";
                var toAddress = "tobiassagis@gmail.com";
                var subject = "Question from " + m.Name;

                // Create a proper body for the email with stringbuilder
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("From: {0}", m.Email.ToString());
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("Message: {0}", m.Description.ToString());
                string message = sb.ToString();

                //start email Thread
                var tEmail = new Thread(() =>
                SendEmail(toAddress, fromAddress, subject, message));
                tEmail.Start();
            }

            // Finalization of the message
            ViewBag.Message = "Message Succesfully Send! Expect an reply within a few workdays.";
            ModelState.Clear();

            return View();
        }

        // Function that handles smtp message
        public void SendEmail(string toAddress, string fromAddress, string subject, string message)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromAddress);
                    mail.To.Add(new MailAddress(toAddress));
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = false;

                    try
                    {
                        using (var smtpClient = new SmtpClient())
                        {
                            smtpClient.Send(mail);
                        }

                    }

                    finally
                    {
                        //dispose the client
                        mail.Dispose();
                    }

                }
            }
            catch (SmtpFailedRecipientsException ex)
            {
                foreach (SmtpFailedRecipientException t in ex.InnerExceptions)
                {
                    var status = t.StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        Response.Write("Delivery failed - retrying in 5 seconds.");
                        System.Threading.Thread.Sleep(5000);
                        //resend
                        //smtpClient.Send(message);
                    }
                    else
                    {
                        string error = String.Format("Failed to deliver message to {0}", t.FailedRecipient);
                        Response.Write(error);
                    }
                }
            }
            catch (SmtpException Se)
            {
                // handle exception here
                Response.Write(Se.ToString());
            }

            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }
    }
}