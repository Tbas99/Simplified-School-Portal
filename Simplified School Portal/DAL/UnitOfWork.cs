using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;

namespace Simplified_School_Portal.DAL
{
    public class UnitOfWork : IDisposable
    {
        private SSPDatabaseEntities context = new SSPDatabaseEntities();
        private GenericRepository<API_package> api_packageRepository;
        private GenericRepository<Info_request> info_requestRepository;
        private GenericRepository<Package_call> package_callRepository;
        private GenericRepository<Feature_request> feature_requestRepository;
        private GenericRepository<Pages> pagesRepository;

        public GenericRepository<API_package> Api_packageRepository
        {
            get
            {
                if (this.api_packageRepository == null)
                {
                    this.api_packageRepository = new GenericRepository<API_package>(context);
                }
                return api_packageRepository;
            }
        }

        public GenericRepository<Info_request> Info_requestRepository
        {
            get
            {
                if (this.info_requestRepository == null)
                {
                    this.info_requestRepository = new GenericRepository<Info_request>(context);
                }
                return info_requestRepository;
            }
        }

        public GenericRepository<Package_call> Package_callRepository
        {
            get
            {
                if (this.package_callRepository == null)
                {
                    this.package_callRepository = new GenericRepository<Package_call>(context);
                }
                return package_callRepository;
            }
        }

        public GenericRepository<Feature_request> Feature_requestRepository
        {
            get
            {
                if (this.feature_requestRepository == null)
                {
                    this.feature_requestRepository = new GenericRepository<Feature_request>(context);
                }
                return feature_requestRepository;
            }
        }

        public GenericRepository<Pages> PagesRepository
        {
            get
            {
                if (this.pagesRepository == null)
                {
                    this.pagesRepository = new GenericRepository<Pages>(context);
                }
                return pagesRepository;
            }
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