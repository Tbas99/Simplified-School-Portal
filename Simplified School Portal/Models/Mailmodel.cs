using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Simplified_School_Portal.Models
{
    public class Mailmodel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

        public bool IsValidEmail()
        {
            try
            {
                var addr = new MailAddress(Email);
                return addr.Address == Email;
            }
            catch
            {
                return false;
            }
        }
    }
}