using OptimalCurrencyExchange.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models.ModelsDB
{
    public class ExchangeRate
    {
        public Guid Id { get; set; }
        public Guid BankId { get; set; }
        public virtual Bank Bank { get; set; }
        public enCurrency CurrencySale { get; set; }
        public enCurrency CurrencyBuy { get; set; }
        public double Rate { get; set; }
    }
}
