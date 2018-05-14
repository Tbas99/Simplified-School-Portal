using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Simplified_School_Portal.Models
{
    public class APImodel
    {
        public string package_name { get; set; }
        public string package_descr { get; set; }
        public string package_url { get; set; }
        public string package_callId { get; set; }

        // For list-purposes
        public IEnumerable<int> SelectedCalls { get; set; }
        public IEnumerable<SelectListItem> Calls { get; set; }
    }
}