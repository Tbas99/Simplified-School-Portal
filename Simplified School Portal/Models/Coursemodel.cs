using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Simplified_School_Portal.Models
{
    public class Coursemodel
    {
        public string room { get; set; }
        public string subject { get; set; }
        public string teacher { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string description { get; set; }

        // seperate time and date
        public string startDate { get; set; }
        public string endDate { get; set; }

    }
}