using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;

namespace Simplified_School_Portal.DAL
{
    public interface IAPI_packageRepository : IDisposable
    {
        IEnumerable<API_package> GetPackages();
        API_package GetApi_PackageById(int api_packageId);
        void InsertApi_package(API_package api_package);
        void DeleteApi_package(int api_packageId);
        void UpdateApi_package(API_package api_package);
        void Save();
    }
}