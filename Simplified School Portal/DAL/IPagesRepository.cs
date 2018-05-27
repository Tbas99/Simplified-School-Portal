using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;

namespace Simplified_School_Portal.DAL
{
    public interface IPagesRepository : IDisposable
    {
        IEnumerable<Pages> GetPages();
        Pages GetPagesById(int pagesId);
        void InsertPages(Pages pages);
        void DeletePages(int pagesId);
        void UpdatePages(Pages pages);
        void Save();
    }
}