using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.Models.ModelsDB
{
    public class Bank
    {
        [Browsable(false)]
        public Guid Id { get; set; }
        [Required, Display(Name = "Название")]
        public string Name { get; set; }
        [Required, Display(Name = "Сайт")]
        public string Url { get; set; }
        public virtual List<ExchangeRate> ExchangeRates { get; set; }
        [Required, Display(Name = "Последнее обновление данных")]
        public DateTime LastUpdate { get; set; }
    }
}
