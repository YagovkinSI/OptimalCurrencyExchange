using OptimalCurrencyExchange.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models.ViewModels
{
    public class ExchangeRequest
    {
        [Required, Display(Name = "Валюта продажи")]
        [EnumDataType(typeof(enCurrency))]
        public enCurrency CurrencyFrom { get; set; }
        [Required, Display(Name = "Количетсво")]
        public double CountFrom { get; set; }
        [Required, Display(Name = "Валюта покупки")]
        [EnumDataType(typeof(enCurrency))]
        public enCurrency CurrencyTo { get; set; }
    }
}
