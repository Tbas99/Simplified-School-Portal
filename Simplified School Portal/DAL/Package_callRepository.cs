using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;
using System.Data.Entity;

namespace Simplified_School_Portal.DAL
{
    public class Package_callRepository : IPackage_callRepository, IDisposable
    {
        private SSPDatabaseEntities context;

        public Package_callRepository(SSPDatabaseEntities context)
        {
            this.context = context;
        }

        public IEnumerable<Package_call> GetPackageCalls()
        {
            return context.Package_call.ToList();
        }

        public Package_call GetPackage_callById(int package_callId)
        {
            return context.Package_call.Find(package_callId);
        }

        public void InsertPackage_call(Package_call package_call)
        {
            context.Package_call.Add(package_call);
        }

        public void DeletePackage_call(int package_callId)
        {
            Package_call package_call = context.Package_call.Find(package_callId);
            context.Package_call.Remove(package_call);
        }

        public void UpdatePackage_call(Package_call package_call)
        {
            context.Entry(package_call).State = EntityState.Modified;
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