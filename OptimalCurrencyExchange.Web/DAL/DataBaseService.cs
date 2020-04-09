using Microsoft.EntityFrameworkCore;
using OptimalCurrencyExchange.Web.Models;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.DAL
{
    public class DataBaseService
    {
        private Exception dataBaseExceptionRead = new Exception("Ошибка чтения из базы данных");
        private Exception dataBaseExceptionWrite = new Exception("Ошибка записи в базу данных");

        private ExchangeDbContext context;

        public DataBaseService (ExchangeDbContext context)
        {
            this.context = context;
        }

        public List<Bank> GetBanks()
        {
            try
            {
                return context.Banks.ToList();
            }
            catch
            {
                throw dataBaseExceptionRead;
            }
        }
        public List<ExchangeRate> GetExchangeRates(Guid bankId)
        {
            try
            {
                return context.ExchangeRates
                    .Where(e => e.BankId == bankId)
                    .ToList();
            }
            catch
            {
                throw dataBaseExceptionRead;
            }
        }

        public Bank GetBank(Guid bankId)
        {
            try
            {
                return context.Banks
                    .FirstOrDefault(e => e.Id == bankId);
            }
            catch
            {
                throw dataBaseExceptionRead;
            }
        }

        public List<ExchangeRate> GetExchangeRates()
        {
            try
            {
                return context.ExchangeRates
                .Include(e => e.Bank)
                .ToList();
            }
            catch
            {
                throw dataBaseExceptionRead;
            }
        }

        public async Task AddBankAsync(Bank bankFromDB)
        {
            try
            {
                context.Banks.Add(bankFromDB);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw dataBaseExceptionWrite;
            }
        }

        public async Task UpdateExchangeRates(Guid bankId, List<ExchangeRate> newExchangeRateList, DateTime updateDate)
        {
            var bankFromDB = GetBank(bankId);
            var ratesFromDB = GetExchangeRates(bankId);
            context.ExchangeRates.RemoveRange(ratesFromDB);
            bankFromDB.ExchangeRates = newExchangeRateList;
            bankFromDB.LastUpdate = updateDate;
            await context.SaveChangesAsync();
        }
    }
}
