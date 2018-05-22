using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;
using System.Data.Entity;

namespace Simplified_School_Portal.DAL
{
    public class API_packageRepository : IAPI_packageRepository, IDisposable
    {
        private SSPDatabaseEntities context;

        public API_packageRepository(SSPDatabaseEntities context)
        {
            this.context = context;
        }

        public IEnumerable<API_package> GetPackages()
        {
            return context.API_package.ToList();
        }

        public API_package GetApi_PackageById(int api_packageId)
        {
            return context.API_package.Find(api_packageId);
        }

        public void InsertApi_package(API_package api_package)
        {
            context.API_package.Add(api_package);
        }

        public void DeleteApi_package(int api_packageId)
        {
            API_package api_package = context.API_package.Find(api_packageId);
            context.API_package.Remove(api_package);
        }

        public void UpdateApi_package(API_package api_package)
        {
            context.Entry(api_package).State = EntityState.Modified;
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