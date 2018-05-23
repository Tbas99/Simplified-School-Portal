using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Simplified_School_Portal.Models
{
    public class CreatePagemodel
    {
        public string x { get; set; }
        public string y { get; set; }
        public string w { get; set; }
        public string h { get; set; }
        public string content { get; set; }
    }
}