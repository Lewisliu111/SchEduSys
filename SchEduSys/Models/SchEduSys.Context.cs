﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SchEduSys.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SchEduSysEntities : DbContext
    {
        public SchEduSysEntities()
            : base("name=SchEduSysEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<admin> admin { get; set; }
        public virtual DbSet<course> course { get; set; }
        public virtual DbSet<courseandtopic> courseandtopic { get; set; }
        public virtual DbSet<coursetopic> coursetopic { get; set; }
        public virtual DbSet<department> department { get; set; }
    }
}
