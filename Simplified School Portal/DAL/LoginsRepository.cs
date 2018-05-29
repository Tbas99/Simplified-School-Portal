using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;
using System.Data.Entity;

namespace Simplified_School_Portal.DAL
{
    public class LoginsRepository : ILoginsRepository, IDisposable
    {
        private SSPDatabaseEntities context;

        public LoginsRepository(SSPDatabaseEntities context)
        {
            this.context = context;
        }

        public IEnumerable<Logins> GetLogins()
        {
            return context.Logins.ToList();
        }

        public Logins GetLoginsById(int loginsId)
        {
            return context.Logins.Find(loginsId);
        }

        public void InsertLogins(Logins logins)
        {
            context.Logins.Add(logins);
        }

        public void DeleteLogins(int loginsId)
        {
            Logins login = context.Logins.Find(loginsId);
            context.Logins.Remove(login);
        }

        public void UpdateLogins(Logins logins)
        {
            context.Entry(logins).State = EntityState.Modified;
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