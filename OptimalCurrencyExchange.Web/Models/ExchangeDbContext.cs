using Microsoft.EntityFrameworkCore;
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
        public DbSet<Bank> Banks { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
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
            CreateBanks(modelBuilder);
            CreateExchangeRates(modelBuilder);
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
            model.HasOne(e => e.Bank)
                .WithMany(e => e.ExchangeSteps)
                .HasForeignKey(e => e.BankId);
        }

        private void CreateBanks(ModelBuilder modelBuilder)
        {
            var model = modelBuilder.Entity<Bank>();
            model.HasIndex(e => e.Id);
            model.HasMany(e => e.ExchangeRates)
                .WithOne(e => e.Bank)
                .HasForeignKey(e => e.BankId)
                .OnDelete(DeleteBehavior.Cascade);
            model.HasMany(e => e.ExchangeSteps)
                .WithOne(e => e.Bank)
                .HasForeignKey(e => e.BankId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void CreateExchangeRates(ModelBuilder modelBuilder)
        {
            var model = modelBuilder.Entity<ExchangeRate>();
            model.HasIndex(e => e.Id);
            model.HasIndex(e => e.BankId);
            model.HasIndex(e => e.CurrencySale);
            model.HasIndex(e => e.CurrencyBuy);
            model.HasIndex(e => new { e.CurrencySale, e.CurrencyBuy });
            model.HasIndex(e => new { e.CurrencySale, e.CurrencyBuy, e.BankId });
            model.HasOne(e => e.Bank)
                .WithMany(e => e.ExchangeRates)
                .HasForeignKey(e => e.BankId);
        }
    }
}
