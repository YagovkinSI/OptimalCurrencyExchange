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
using System.Text;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.BLL
{
    public class ExchangeService
    {
        private readonly DataBaseService dataBaseService;

        private readonly HttpClient client ;

        private IBankInformer[] bankInformers = new IBankInformer[] {
            new AlfaBankInformer(),
            new PrivatBankInformer(), 
            new BelarusBankInformer() };

        const double DataRelevanceDeltaHours = 1.0;

        public ExchangeService(DataBaseService dataBaseService, HttpClient httpClient)
        {
            this.dataBaseService = dataBaseService;
            client = httpClient;
        }

        /// <summary>
        /// Проверка акктуальности данных курсов обмена в БД
        /// </summary>
        /// <param name="forcedUpdate">Принудительно обновить данные</param>
        /// <returns>Отчет о успешности и времени последнего обновления для каждого банка</returns>
        public async Task<List<string>> CheckDataRelevanceAsync(bool forcedUpdate = false)
        {
            var result = new List<string>();
            var banksFromDB = dataBaseService.GetBanks();
            foreach (var bankInformer in bankInformers)
            {                
                var bankFromDB = banksFromDB.SingleOrDefault(b => b.Name == bankInformer.Name);
                if (bankFromDB == null)
                {
                    bankFromDB = new Bank
                    {
                        Name = bankInformer.Name,
                        Url = bankInformer.Url,
                        LastUpdate = DateTime.MinValue
                    };
                    await dataBaseService.AddBankAsync(bankFromDB);
                }
                if (forcedUpdate || bankFromDB.LastUpdate < DateTime.Now - TimeSpan.FromHours(DataRelevanceDeltaHours))
                {
                    try
                    {
                        await UpdateDataAsync(bankFromDB, bankInformer);
                    }
                    catch (Exception ex)
                    {
                        result.Add(ex.Message);
                    }
                }
                result.Add($"Последнее обновление курса банка {bankFromDB.Name} - {bankFromDB.LastUpdate}.");
            }
            return result;
        }

        /// <summary>
        /// Перебор всех возможных вариантов обмена с получение лучших вариантов
        /// </summary>
        /// <param name="exchangeRequest">Запрос на обмен валюты</param>
        /// <returns>Отсортированный список всех возможных вариантов обменов</returns>
        internal List<Exchange> FindBestExchanges(ExchangeRequest exchangeRequest)
        {
            var exchangeDbContext = dataBaseService.GetExchangeRates();
            var task = new FindAllExchangesTask(exchangeRequest, exchangeDbContext);
            task.Exucute();
            var allExchange = task.Result;
            return allExchange;
        }

        /// <summary>
        /// Обновление данных банка в БД
        /// </summary>
        /// <param name="bankFromDB">Банк</param>
        /// <param name="bankInformer">Класс работы с банком</param>
        /// <returns></returns>
        private async Task UpdateDataAsync(Bank bankFromDB, IBankInformer bankInformer)
        {
            var updateDate = DateTime.Now;
            var newExchangeRateList = await bankInformer.GetNewExchangeRateListAsync(client);
            if (newExchangeRateList.Count > 0)
            {
                await dataBaseService.UpdateExchangeRates(bankFromDB.Id, newExchangeRateList, updateDate);
            }
        }
    }
}
