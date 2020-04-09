using Microsoft.EntityFrameworkCore;
using OptimalCurrencyExchange.BLL;
using OptimalCurrencyExchange.Web.BLL.BankConverters;
using OptimalCurrencyExchange.Web.Models;
using OptimalCurrencyExchange.Web.Models.Enums;
using OptimalCurrencyExchange.Web.Models.ModelsBL;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using OptimalCurrencyExchange.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.BLL
{
    public static class ExchangeHelper
    {
        private static HttpClient client = new HttpClient();

        private static IBankInformer[] bankConverters = new IBankInformer[] {
            new AlfaBankInformer(),
            new PrivatBankInformer(), 
            new BelarusBankInformer() };

        const double DataRelevanceDeltaHours = 1.0;

        public static async Task CheckDataRelevanceAsync(ExchangeDbContext context)
        {
            var banksFromDB = context.Banks.ToList();
            foreach (var bankConverter in bankConverters)
            {
                var bankFromDB = banksFromDB.SingleOrDefault(b => b.Name == bankConverter.Name);
                if (bankFromDB == null)
                {
                    bankFromDB = new Bank
                    {
                        Name = bankConverter.Name,
                        Url = bankConverter.Url,
                        LastUpdate = DateTime.MinValue
                    };
                    context.Banks.Add(bankFromDB);
                    await context.SaveChangesAsync();
                }
                if (bankFromDB.LastUpdate < DateTime.Now - TimeSpan.FromHours(DataRelevanceDeltaHours))
                {
                    await UpdateDataAsync(context, bankFromDB, bankConverter);
                }                
            }
        }

        internal static List<Exchange> FindBestExchanges(ExchangeDbContext context, ExchangeRequest exchangeRequest)
        {
            var exchangeDbContext = context.ExchangeRates.ToList();
            var task = new FindAllExchangesTask(exchangeRequest, exchangeDbContext);
            task.Exucute();
            var allExchange = task.Result;
            return allExchange;
        }

        private static async Task UpdateDataAsync(ExchangeDbContext context, Bank bankFromDB, IBankInformer bankConverter)
        {
            var updateDate = DateTime.Now;
            var newExchangeRateList = await bankConverter.GetNewExchangeRateListAsync(client);
            if (newExchangeRateList.Count > 0)
            {
                var ratesFromDB = context.ExchangeRates.Where(r => r.BankId == bankFromDB.Id);
                context.ExchangeRates.RemoveRange(ratesFromDB);
                bankFromDB.ExchangeRates = newExchangeRateList;
                bankFromDB.LastUpdate = updateDate;
                await context.SaveChangesAsync();
            }
        }

        public static void Fill(this ExchangeRate obj, enCurrency sale, enCurrency buy, decimal rate)
        {
            obj.CurrencySale = sale;
            obj.CurrencyBuy = buy;
            obj.Rate = (double)rate;
        }
    }
}
