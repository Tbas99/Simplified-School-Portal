//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Simplified_School_Portal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Feature_request
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Feature_request()
        {
            this.Request = new HashSet<Request>();
        }
    
        public int Feature_requestId { get; set; }
        public string Feature_name { get; set; }
        public string Feature_develop_url { get; set; }
        public string Feature_request_issuer { get; set; }
        public System.DateTime Feature_request_date { get; set; }
        public string Feature_is_implemented { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> Request { get; set; }
    }
}
