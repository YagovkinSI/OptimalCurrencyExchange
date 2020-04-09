using OptimalCurrencyExchange.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models.ModelsBL
{
    public class Exchange
    {
        [Browsable(false)]
        public Guid Id { get; set; }
        [Required, Display(Name = "Дата обмена")]
        public DateTime DateTime { get; set; }
        [Required, Display(Name = "Валюта продажи")]
        [EnumDataType(typeof(enCurrency))]
        public enCurrency CurrencyFrom { get; set; }
        [Required, Display(Name = "Количетсво проданной валюты")]
        public double CountFrom { get; set; }
        [Required, Display(Name = "Валюта покупки")]
        [EnumDataType(typeof(enCurrency))]
        public enCurrency CurrencyTo { get; set; }
        [Required, Display(Name = "Количетсво полученной валюты")]
        public double CountTo { get; set; }
        public virtual List<ExchangeStep> ExchangeSteps { get; set; }
    }
}
