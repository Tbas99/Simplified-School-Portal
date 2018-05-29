using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Simplified_School_Portal.Models
{
    public class Statisticsmodel
    {
        public string loginId { get; set; }
        public DateTime date { get; set; }
        public TimeSpan timestamp { get; set; }
    }
}