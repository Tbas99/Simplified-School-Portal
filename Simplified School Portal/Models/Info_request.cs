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
    
    public partial class Info_request
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Info_request()
        {
            this.Request = new HashSet<Request>();
        }
    
        public int Info_requestId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Request_user { get; set; }
        public System.DateTime Request_date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> Request { get; set; }
    }
}
