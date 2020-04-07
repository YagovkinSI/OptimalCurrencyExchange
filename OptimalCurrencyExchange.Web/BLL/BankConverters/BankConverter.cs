using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.BLL.BankConverters
{
    public abstract class BankConverter : IBankConverter
    {
        public abstract string Name { get; }
        public abstract string Url { get; }
        public abstract Task<List<ExchangeRate>> GetNewExchangeRateListAsync(HttpClient client);
    }
}
