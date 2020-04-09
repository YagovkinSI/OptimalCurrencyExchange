using Microsoft.EntityFrameworkCore;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OptimalCurrencyExchange.Web.Models.ModelsBL;

namespace OptimalCurrencyExchange.Web.Models
{
    public class ExchangeDbContext : DbContext
    {
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
            CreateBanks(modelBuilder);
            CreateExchangeRates(modelBuilder);
        }        

        private void CreateBanks(ModelBuilder modelBuilder)
        {
            var model = modelBuilder.Entity<Bank>();
            model.HasIndex(e => e.Id);
            model.HasMany(e => e.ExchangeRates)
                .WithOne(e => e.Bank)
                .HasForeignKey(e => e.BankId)
                .OnDelete(DeleteBehavior.Cascade);
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
