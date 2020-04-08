using OptimalCurrencyExchange.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models.ModelsDB
{
    public class ExchangeRate
    {
        [Browsable(false)]
        public Guid Id { get; set; }
        [Browsable(false)]
        public Guid BankId { get; set; }
        public virtual Bank Bank { get; set; }
        [Required, Display(Name = "Валюта продажи")]
        [EnumDataType(typeof(enCurrency))]
        public enCurrency CurrencySale { get; set; }
        [Required, Display(Name = "Валюта покупки")]
        [EnumDataType(typeof(enCurrency))]
        public enCurrency CurrencyBuy { get; set; }
        [Required, Display(Name = "Курс за одну единицы валюты покупки")]
        public double Rate { get; set; }
    }
}
