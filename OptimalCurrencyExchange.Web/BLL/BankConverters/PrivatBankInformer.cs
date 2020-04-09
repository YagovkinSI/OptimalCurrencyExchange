using OptimalCurrencyExchange.Web.Models.Enums;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.BLL.BankConverters
{
    public class PrivatBankInformer : BankInformer
    {
        public override string Name => "ПриватБанк";
        public override string Url => "https://privatbank.ua/";

        protected override string ApiPath => "https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5";

        public override async Task<List<ExchangeRate>> GetNewExchangeRateListAsync(HttpClient client)
        {
            var bankObj = await GetRateObject<PrivatBankObj[]>(client);
            var list = GetExchangeRates(bankObj);
            return list;
            
        }

        protected override List<ExchangeRate> GetExchangeRates<T>(T bankObj)
        {
            var objArray = bankObj as PrivatBankObj[];
            var list = new List<ExchangeRate>();
            if (objArray.Length == 0)
                return list;
            foreach (var obj in objArray)
            {
                var ccy1 = GetCurrency(obj.ccy);
                var ccy2 = GetCurrency(obj.base_ccy);
                if (ccy1 == enCurrency.Unknow || ccy2 == enCurrency.Unknow)
                    continue;
                if (obj.buy > 0)
                    AddToList(ref list, ccy1, ccy2, 1 / obj.buy);
                if (obj.sale > 0)
                    AddToList(ref list, ccy2, ccy1, obj.sale);
            }
            return list;
        }

        private enCurrency GetCurrency(string currency)
        {
            switch(currency)
            {
                case "RUR":
                    return enCurrency.RUR;
                case "USD":
                    return enCurrency.USD;
                case "EUR":
                    return enCurrency.EUR;
                case "UAH":
                    return enCurrency.UAH;
                default:
                    return enCurrency.Unknow;
            }
        }

        private class PrivatBankObj
        {
            public string ccy { get; set; }
            public string base_ccy { get; set; }
            public decimal buy { get; set; }
            public decimal sale { get; set; }
        }
    }
}
