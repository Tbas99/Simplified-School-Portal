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
    
    public partial class Page_position
    {
        public int Page_positionId { get; set; }
        public string Page_position_column { get; set; }
        public int Package_callId { get; set; }
    
        public virtual Package_call Package_call { get; set; }
    }
}