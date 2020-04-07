using OptimalCurrencyExchange.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models.ModelsDB
{
    public class Exchange
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public enCurrency CurrencyFrom { get; set; }
        public double CountFrom { get; set; }
        public enCurrency CurrencyTo { get; set; }
        public double CountTo { get; set; }
        public virtual List<ExchangeStep> ExchangeSteps { get; set; }
    }
}
