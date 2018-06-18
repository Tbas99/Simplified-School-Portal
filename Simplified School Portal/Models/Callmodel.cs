using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Simplified_School_Portal.Models
{
    public class Callmodel
    {
        public string call { get; set; }
        public string call_url { get; set; }
        public string call_auth_needed { get; set; }
        public string call_type { get; set; }
        public string call_data_section { get; set; }
        public string call_content_key { get; set; }
    }
}