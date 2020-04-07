using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models.ModelsDB
{
    public class Bank
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public virtual List<ExchangeRate> ExchangeRates { get; set; }
        public DateTime LastUpdate { get; set; }

    }
}
