using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simplified_School_Portal.Models;

namespace Simplified_School_Portal.DAL
{
    public interface IInfo_requestRepository : IDisposable
    {
        IEnumerable<Info_request> GetInfoRequests();
        Info_request GetInfo_requestById(int info_requestId);
        void InsertInfo_request(Info_request info_request);
        void DeleteInfo_request(int info_requestId);
        void UpdateInfo_request(Info_request info_request);
        void Save();
    }
}