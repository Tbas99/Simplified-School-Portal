using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Simplified_School_Portal.Models;

namespace Simplified_School_Portal.Models
{
    public class CustomView : IView
    {
        public string htmlContent { get; set; }

        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        { 
            writer.WriteLine(htmlContent);
        }

    }
}