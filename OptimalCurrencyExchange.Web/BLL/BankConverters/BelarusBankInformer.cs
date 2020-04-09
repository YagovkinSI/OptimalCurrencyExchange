using OptimalCurrencyExchange.Web.Models.Enums;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.BLL.BankConverters
{
    public class BelarusBankInformer : BankInformer
    {
        public override string Name => "Беларусбанк";
        public override string Url => "https://belarusbank.by/";

        protected override string ApiPath => "https://belarusbank.by/api/kursExchange?city=%D0%9C%D0%B8%D0%BD%D1%81%D0%BA";

        public override async Task<List<ExchangeRate>> GetNewExchangeRateListAsync(HttpClient client)
        {
            var bankObj = await GetRateObject<BelarusBankObj[]>(client);
            var list = GetExchangeRates(bankObj);
            return list;
        }

        protected override List<ExchangeRate> GetExchangeRates<T>(T bankObj)
        {
            var objArray = bankObj as BelarusBankObj[];
            var list = new List<ExchangeRate>();
            if (objArray.Length == 0)
                return list;
            var obj = objArray[0];
            if (obj.USD_in > 0)
                AddToList(ref list, enCurrency.USD, enCurrency.BYN, 1 / obj.USD_in);
            if (obj.USD_out > 0)
                AddToList(ref list, enCurrency.BYN, enCurrency.USD, obj.USD_out);
            if (obj.EUR_in > 0)
                AddToList(ref list, enCurrency.EUR, enCurrency.BYN, 1 / obj.EUR_in);
            if (obj.EUR_out > 0)
                AddToList(ref list, enCurrency.BYN, enCurrency.EUR, obj.EUR_out);
            if (obj.RUB_in > 0)
                AddToList(ref list, enCurrency.RUR, enCurrency.BYN, 100 / obj.RUB_in);
            if (obj.RUB_out > 0)
                AddToList(ref list, enCurrency.BYN, enCurrency.RUR, obj.RUB_out / 100);
            if (obj.UAH_in > 0)
                AddToList(ref list, enCurrency.UAH, enCurrency.BYN, 100 / obj.UAH_in);
            if (obj.UAH_out > 0)
                AddToList(ref list, enCurrency.BYN, enCurrency.UAH, obj.UAH_out / 100);
            if (obj.USD_EUR_in > 0)
                AddToList(ref list, enCurrency.USD, enCurrency.EUR, 1 / obj.USD_EUR_in);
            if (obj.USD_EUR_out > 0)
                AddToList(ref list, enCurrency.EUR, enCurrency.USD, 1 / obj.USD_EUR_out);
            if (obj.USD_RUB_in > 0)
                AddToList(ref list, enCurrency.USD, enCurrency.RUR, 1 / obj.USD_RUB_in);
            if (obj.USD_RUB_out > 0)
                AddToList(ref list, enCurrency.RUR, enCurrency.USD, 1 / obj.USD_RUB_out);
            if (obj.RUB_EUR_in > 0)
                AddToList(ref list, enCurrency.RUR, enCurrency.EUR, 1 / obj.RUB_EUR_in);
            if (obj.RUB_EUR_out > 0)
                AddToList(ref list, enCurrency.EUR, enCurrency.RUR, 1 / obj.RUB_EUR_out);
            return list;
        }

        private class BelarusBankObj
        {
            public decimal USD_in { get; set; }
            public decimal USD_out { get; set; }
            public decimal EUR_in { get; set; }
            public decimal EUR_out { get; set; }
            public decimal RUB_in { get; set; } //x100
            public decimal RUB_out { get; set; } //x100
            public decimal UAH_in { get; set; } //x100
            public decimal UAH_out { get; set; } //x100
            public decimal USD_EUR_in { get; set; }
            public decimal USD_EUR_out { get; set; }
            public decimal USD_RUB_in { get; set; }
            public decimal USD_RUB_out { get; set; }
            public decimal RUB_EUR_in { get; set; }
            public decimal RUB_EUR_out { get; set; }
        }
    }
}
