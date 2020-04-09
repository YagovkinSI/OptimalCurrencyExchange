using OptimalCurrencyExchange.Web.Models.Enums;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.BLL.BankConverters
{
    public abstract class BankInformer : IBankInformer
    {
        public abstract string Name { get; }
        public abstract string Url { get; }
        protected abstract string ApiPath { get; }
        private string ResponsiveMessageErrorText => $"Не удалось получить обменный курс банка {Name}";
        private string MessageFormatErrorText => $"Неизвестный формат данных обменного курса банка {Name}";

        /// <summary>
        /// Получение свежего курса обмена валюты 
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Список курсов обмена валюты</returns>
        public abstract Task<List<ExchangeRate>> GetNewExchangeRateListAsync(HttpClient client);

        protected async Task<T> GetRateObject<T>(HttpClient client)
        {
            var responseMessage = await GetResponseMessage(client);
            T bankObject;
            try
            {                
                bankObject = await responseMessage.Content.ReadAsAsync<T>();
            }
            catch
            {
                throw new Exception(MessageFormatErrorText);
            }

            return bankObject;
        }

        /// <summary>
        /// Получение свежего курса обмена от API банка
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Ответ API банка</returns>
        private async Task<HttpResponseMessage> GetResponseMessage(HttpClient client)
        {
            HttpResponseMessage responseMessage;
            try
            {
                responseMessage = await client.GetAsync(ApiPath);
            }
            catch
            {
                throw new Exception(ResponsiveMessageErrorText);
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new Exception(ResponsiveMessageErrorText);
            }

            return responseMessage;
        }

        /// <summary>
        /// Приведение полученного ответа API банка в формат списка курсов БД
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bankObj"></param>
        /// <returns>Список обемнных курсов</returns>
        protected abstract List<ExchangeRate> GetExchangeRates<T>(T bankObj);  

        protected void AddToList(ref List<ExchangeRate> list, enCurrency sale, enCurrency buy, decimal rate)
        {
            var obj = new ExchangeRate
            {
                CurrencySale = sale,
                CurrencyBuy = buy,
                Rate = (double)rate
            };
            list.Add(obj);
        }
    }
}
