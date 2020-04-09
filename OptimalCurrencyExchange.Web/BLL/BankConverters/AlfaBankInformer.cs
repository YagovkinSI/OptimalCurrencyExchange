using OptimalCurrencyExchange.Web.Models.Enums;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.BLL.BankConverters
{
    public class AlfaBankInformer : BankInformer
    {
        public override string Name => "Альфа банк";
        public override string Url => "https://www.alfabank.by/";

        protected override string ApiPath => "https://developerhub.alfabank.by:8273/partner/1.0.1/public/rates";

        public override async Task<List<ExchangeRate>> GetNewExchangeRateListAsync(HttpClient client)
        {
            var bankObj = await GetRateObject<AlfaBankObj>(client);
            var list = GetExchangeRates(bankObj);
            return list;
        }

        protected override List<ExchangeRate> GetExchangeRates<T>(T bankObj)
        {
            var alfaBankObj = bankObj as AlfaBankObj;
            var list = new List<ExchangeRate>();
            foreach (var obj in alfaBankObj.Rates)
            {
                var ccy1 = GetCurrency(obj.buyIso);
                var ccy2 = GetCurrency(obj.sellIso);
                if (ccy1 == enCurrency.Unknow || ccy2 == enCurrency.Unknow)
                    continue;
                if (obj.buyRate > 0)
                    AddToList(ref list, ccy1, ccy2, obj.buyRate / obj.quantity);
                if (obj.sellRate > 0)
                    AddToList(ref list, ccy2, ccy1, 1 / obj.sellRate * obj.quantity);
            }
            return list;
        }

        private enCurrency GetCurrency(string currency)
        {
            switch(currency)
            {
                case "RUB":
                    return enCurrency.RUR;
                case "USD":
                    return enCurrency.USD;
                case "EUR":
                    return enCurrency.EUR;
                case "UAH":
                    return enCurrency.UAH; 
                case "BYN":
                    return enCurrency.BYN;
                default:
                    return enCurrency.Unknow;
            }
        }

        private class AlfaBankObj
        {
            public AlfaBankRate[] Rates { get; set; }
        }

        private class AlfaBankRate
        {
            public string buyIso { get; set; }
            public string sellIso { get; set; }
            public decimal buyRate { get; set; }
            public decimal sellRate { get; set; }
            public int quantity { get; set; }
            public DateTime date { get; set; }
        }
    }
}
