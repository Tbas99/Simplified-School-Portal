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
    
    public partial class Package_call
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Package_call()
        {
            this.API_package = new HashSet<API_package>();
        }
    
        public int Package_callId { get; set; }
        public string Call { get; set; }
        public string Call_url { get; set; }
        public string Call_verificationNeeded { get; set; }
        public string Call_type { get; set; }
        public string Call_data_section { get; set; }
        public string Call_content_key { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<API_package> API_package { get; set; }
    }
}
