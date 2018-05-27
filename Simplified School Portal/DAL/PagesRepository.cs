using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;
using System.Data.Entity;

namespace Simplified_School_Portal.DAL
{
    public class PagesRepository :IPagesRepository, IDisposable
    {
        private SSPDatabaseEntities context;

        public PagesRepository(SSPDatabaseEntities context)
        {
            this.context = context;
        }

        public IEnumerable<Pages> GetPages()
        {
            return context.Pages.ToList();
        }

        public Pages GetPagesById(int pagesId)
        {
            return context.Pages.Find(pagesId);
        }

        public void InsertPages(Pages pages)
        {
            context.Pages.Add(pages);
        }

        public void DeletePages(int pagesId)
        {
            Pages page = context.Pages.Find(pagesId);
            context.Pages.Remove(page);
        }

        public void UpdatePages(Pages pages)
        {
            context.Entry(pages).State = EntityState.Modified;
        }
        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}