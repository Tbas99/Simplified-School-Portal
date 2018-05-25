using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;
using System.Data.Entity;


namespace Simplified_School_Portal.DAL
{
    public class Feature_requestRepository : IFeature_requestRepository, IDisposable
    {
        private SSPDatabaseEntities context;

        public Feature_requestRepository(SSPDatabaseEntities context)
        {
            this.context = context;
        }

        public IEnumerable<Feature_request> GetFeatureRequests()
        {
            return context.Feature_request.ToList();
        }

        public Feature_request GetFeature_requestById(int feature_requestId)
        {
            return context.Feature_request.Find(feature_requestId);
        }

        public void InsertFeature_request(Feature_request feature_request)
        {
            context.Feature_request.Add(feature_request);
        }

        public void DeleteFeature_request(int feature_requestId)
        {
            Feature_request feature_request = context.Feature_request.Find(feature_requestId);
            context.Feature_request.Remove(feature_request);
        }

        public void UpdateFeature_request(Feature_request feature_request)
        {
            context.Entry(feature_request).State = EntityState.Modified;
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