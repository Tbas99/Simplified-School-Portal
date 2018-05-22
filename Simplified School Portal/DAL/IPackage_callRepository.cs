using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;

namespace Simplified_School_Portal.DAL
{
    public interface IPackage_callRepository : IDisposable
    {
        IEnumerable<Package_call> GetPackageCalls();
        Package_call GetPackage_callById(int package_callId);
        void InsertPackage_call(Package_call package_call);
        void DeletePackage_call(int package_callId);
        void UpdatePackage_call(Package_call package_call);
        void Save();
    }
}