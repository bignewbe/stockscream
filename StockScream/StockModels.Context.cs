﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StockScream
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class StockDbContext : DbContext
    {
        public StockDbContext()
            : base("name=StockDbContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Exchanx> Exchanges { get; set; }
        public virtual DbSet<IndustryFinancial> IndustryFinancials { get; set; }
        public virtual DbSet<PPIndustry> PPIndustries { get; set; }
        public virtual DbSet<PPSector> PPSectors { get; set; }
        public virtual DbSet<PPStock> PPStocks { get; set; }
        public virtual DbSet<SectorFinancial> SectorFinancials { get; set; }
        public virtual DbSet<SelectedStock> SelectedStocks { get; set; }
        public virtual DbSet<StockAnalyst> StockAnalysts { get; set; }
        public virtual DbSet<StockFinancial> StockFinancials { get; set; }
    }
}