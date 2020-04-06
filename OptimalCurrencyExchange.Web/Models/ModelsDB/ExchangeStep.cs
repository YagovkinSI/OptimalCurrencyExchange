using OptimalCurrencyExchange.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models.ModelsDB
{
    public class ExchangeStep
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public Currency CurrencyFrom { get; set; }
        public double CountFrom { get; set; }
        public Currency CurrencyTo { get; set; }
        public double CountTo { get; set; }
        public virtual Guid ExchangeId { get; set; }
        public virtual Exchange Exchange { get; set; }
    }
}
