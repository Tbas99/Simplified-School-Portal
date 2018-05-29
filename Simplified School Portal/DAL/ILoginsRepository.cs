using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;

namespace Simplified_School_Portal.DAL
{
    public interface ILoginsRepository : IDisposable
    {
        IEnumerable<Logins> GetLogins();
        Logins GetLoginsById(int loginsId);
        void InsertLogins(Logins logins);
        void DeleteLogins(int loginsId);
        void UpdateLogins(Logins logins);
        void Save();
    }
}