using OptimalCurrencyExchange.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models.ModelsDB
{
    public class ExchangeStep
    {
        public Guid Id { get; set; }
        [EnumDataType(typeof(enCurrency))]
        public enCurrency CurrencyFrom { get; set; }
        public double CountFrom { get; set; }
        [EnumDataType(typeof(enCurrency))]
        public enCurrency CurrencyTo { get; set; }
        public double CountTo { get; set; }
        public Guid BankId { get; set; }
        public virtual Bank Bank { get; set; }

        public virtual Guid ExchangeId { get; set; }
        public virtual Exchange Exchange { get; set; }
    }
}
