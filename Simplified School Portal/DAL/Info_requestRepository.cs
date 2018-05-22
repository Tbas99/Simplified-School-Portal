using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;
using System.Data.Entity;

namespace Simplified_School_Portal.DAL
{
    public class Info_requestRepository : IInfo_requestRepository, IDisposable
    {
        private SSPDatabaseEntities context;

        public Info_requestRepository(SSPDatabaseEntities context)
        {
            this.context = context;
        }

        public IEnumerable<Info_request> GetInfoRequests()
        {
            return context.Info_request.ToList();
        }

        public Info_request GetInfo_requestById(int info_requestId)
        {
            return context.Info_request.Find(info_requestId);
        }

        public void InsertInfo_request(Info_request info_request)
        {
            context.Info_request.Add(info_request);
        }

        public void DeleteInfo_request(int info_requestId)
        {
            Info_request info_request = context.Info_request.Find(info_requestId);
            context.Info_request.Remove(info_request);
        }

        public void UpdateInfo_request(Info_request info_request)
        {
            context.Entry(info_request).State = EntityState.Modified;
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