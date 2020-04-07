using OptimalCurrencyExchange.Web.BLL.BankConverters;
using OptimalCurrencyExchange.Web.Models;
using OptimalCurrencyExchange.Web.Models.Enums;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
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

        private static IBankConverter[] bankConverters = new IBankConverter[] {
            new AlfaBankConverter(),
            new PrivatBankConverter(), 
            new BelarusBankConverter() };

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

        private static async Task UpdateDataAsync(ExchangeDbContext context, Bank bankFromDB, IBankConverter bankConverter)
        {
            var newExchangeRateList = await bankConverter.GetNewExchangeRateListAsync(client);
            if (newExchangeRateList.Count > 0)
            {
                var ratesFromDB = context.ExchangeRates.Where(r => r.BankId == bankFromDB.Id);
                context.ExchangeRates.RemoveRange(ratesFromDB);
                bankFromDB.ExchangeRates = newExchangeRateList;
                await context.SaveChangesAsync();
            }
        }

        //public static async Task<List<CurrencyEdge>> GetPrivatCurrencyEdge()
        //{
        //    var converting = new List<CurrencyEdge>();
        //    foreach (var converter in bankConverters)
        //    {
        //        var list = await converter.GetCurrencyEdgesAsync(client);
        //        foreach (var edge in list)
        //        {
        //            var existEdge = converting.SingleOrDefault(e => e.Buy == edge.Buy && e.Sale == edge.Sale);
        //            if (existEdge != null)
        //            {
        //                existEdge.Rates.AddRange(edge.Rates);
        //            }
        //            else converting.Add(edge);
        //        }
        //    }
        //    return converting;
        //}

        public static void Fill(this ExchangeRate obj, enCurrency sale, enCurrency buy, decimal rate)
        {
            obj.CurrencySale = sale;
            obj.CurrencyBuy = buy;
            obj.Rate = (double)rate;
        }
    }
}
