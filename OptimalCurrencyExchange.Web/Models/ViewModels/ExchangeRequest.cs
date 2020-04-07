using OptimalCurrencyExchange.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models.ViewModels
{
    public class ExchangeRequest
    {
        public enCurrency CurrencyFrom { get; set; }
        public double CountFrom { get; set; }
        public enCurrency CurrencyTo { get; set; }
    }
}
