using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplified_School_Portal.Models;

namespace Simplified_School_Portal.DAL
{
    public interface IFeature_requestRepository : IDisposable
    {
        IEnumerable<Feature_request> GetFeatureRequests();
        Feature_request GetFeature_requestById(int feature_requestId);
        void InsertFeature_request(Feature_request feature_request);
        void DeleteFeature_request(int feature_requestId);
        void UpdateFeature_request(Feature_request feature_request);
        void Save();

    }
}
