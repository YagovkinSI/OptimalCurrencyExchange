using OptimalCurrencyExchange.Web.Models.Enums;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.BLL.BankConverters
{
    public class AlfaBankConverter : BankConverter
    {
        public override string Name => "Альфа банк";
        public override string Url => "https://www.alfabank.by/";

        private string apiPath = "https://developerhub.alfabank.by:8273/partner/1.0.1/public/rates";

        public override async Task<List<ExchangeRate>> GetNewExchangeRateListAsync(HttpClient client)
        {
            var list = new List<ExchangeRate>();
            var objArray = await GetPrivatBankObjAsync(client);
            if (objArray == null)
                return list;

            foreach (var obj in objArray.Rates)
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

        private void AddToList(ref List<ExchangeRate> list, enCurrency sale, enCurrency buy, decimal rate)
        {
            var obj = new ExchangeRate();
            obj.Fill(sale, buy, rate);
            list.Add(obj);
        }

        private async Task<AlfaBankObj> GetPrivatBankObjAsync(HttpClient client)
        {
            AlfaBankObj bankObjectArray = null;
            var responseMessage = await client.GetAsync(apiPath);
            if (responseMessage.IsSuccessStatusCode)
            {
                bankObjectArray = await responseMessage.Content.ReadAsAsync<AlfaBankObj>();
            }
            return bankObjectArray;
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
