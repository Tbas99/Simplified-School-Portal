﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SSPDatabaseEntities : DbContext
    {
        public SSPDatabaseEntities()
            : base("name=SSPDatabaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<API_package> API_package { get; set; }
        public virtual DbSet<Feature_request> Feature_request { get; set; }
        public virtual DbSet<Info_request> Info_request { get; set; }
        public virtual DbSet<Package_call> Package_call { get; set; }
        public virtual DbSet<Page_position> Page_position { get; set; }
        public virtual DbSet<Request> Request { get; set; }
    }
}
