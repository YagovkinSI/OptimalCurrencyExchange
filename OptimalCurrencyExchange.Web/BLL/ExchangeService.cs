using Microsoft.EntityFrameworkCore;
using OptimalCurrencyExchange.BLL;
using OptimalCurrencyExchange.Web.BLL.BankConverters;
using OptimalCurrencyExchange.Web.DAL;
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
    public class ExchangeService
    {
        private readonly DataBaseService dataBaseService;

        private readonly HttpClient client ;

        private IBankInformer[] bankConverters = new IBankInformer[] {
            new AlfaBankInformer(),
            new PrivatBankInformer(), 
            new BelarusBankInformer() };

        const double DataRelevanceDeltaHours = 1.0;

        public ExchangeService(DataBaseService dataBaseService, HttpClient httpClient)
        {
            this.dataBaseService = dataBaseService;
            client = httpClient;
        }

        public async Task CheckDataRelevanceAsync()
        {
            var banksFromDB = dataBaseService.GetBanks();
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
                    await dataBaseService.AddBankAsync(bankFromDB);
                }
                if (bankFromDB.LastUpdate < DateTime.Now - TimeSpan.FromHours(DataRelevanceDeltaHours))
                {
                    await UpdateDataAsync(bankFromDB, bankConverter);
                }                
            }
        }

        internal List<Exchange> FindBestExchanges(ExchangeRequest exchangeRequest)
        {
            var exchangeDbContext = dataBaseService.GetExchangeRates();
            var task = new FindAllExchangesTask(exchangeRequest, exchangeDbContext);
            task.Exucute();
            var allExchange = task.Result;
            return allExchange;
        }

        private async Task UpdateDataAsync(Bank bankFromDB, IBankInformer bankConverter)
        {
            var updateDate = DateTime.Now;
            var newExchangeRateList = await bankConverter.GetNewExchangeRateListAsync(client);
            if (newExchangeRateList.Count > 0)
            {
                await dataBaseService.UpdateExchangeRates(bankFromDB.Id, newExchangeRateList, updateDate);
            }
        }
    }
}
