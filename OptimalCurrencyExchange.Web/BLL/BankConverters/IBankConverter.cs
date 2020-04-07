using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.BLL.BankConverters
{
    public interface IBankConverter
    {
        string Name { get; }
        string Url { get; }

        Task<List<ExchangeRate>> GetNewExchangeRateListAsync(HttpClient client);
    }
}
