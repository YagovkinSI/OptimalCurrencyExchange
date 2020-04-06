﻿using Microsoft.EntityFrameworkCore;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models
{
    public class ExchangeDbContext : DbContext
    {
        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<ExchangeStep> ExchangeSteps { get; set; }
        public ExchangeDbContext(DbContextOptions<ExchangeDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            CreateExchange(modelBuilder);
            CreateExchangeStep(modelBuilder);
        }

        private void CreateExchange(ModelBuilder modelBuilder)
        {
            var model = modelBuilder.Entity<Exchange>();
            model.HasIndex(e => e.Id);
            model.HasMany(e => e.ExchangeSteps)
                .WithOne(e => e.Exchange)
                .HasForeignKey(e => e.ExchangeId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void CreateExchangeStep(ModelBuilder modelBuilder)
        {
            var model = modelBuilder.Entity<ExchangeStep>();
            model.HasIndex(e => e.Id);
            model.HasOne(e => e.Exchange)
                .WithMany(e => e.ExchangeSteps)
                .HasForeignKey(e => e.ExchangeId);
        }
    }
}