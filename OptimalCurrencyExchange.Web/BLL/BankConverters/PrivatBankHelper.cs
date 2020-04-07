using OptimalCurrencyExchange.Web.Models.Enums;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.Web.BLL.BankConverters
{
    public class PrivatBankConverter : BankConverter
    {
        public override string Name => "ПриватБанк";
        public override string Url => "https://privatbank.ua/";

        private string apiPath = "https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5";

        public override async Task<List<ExchangeRate>> GetNewExchangeRateListAsync(HttpClient client)
        {
            var list = new List<ExchangeRate>();
            var objArray = await GetPrivatBankObjAsync(client);
            if (objArray == null)
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

        private void AddToList(ref List<ExchangeRate> list, enCurrency sale, enCurrency buy, decimal rate)
        {
            var obj = new ExchangeRate();
            obj.Fill(sale, buy, rate);
            list.Add(obj);
        }

        private async Task<PrivatBankObj[]> GetPrivatBankObjAsync(HttpClient client)
        {
            PrivatBankObj[] bankObjectArray = null;
            var responseMessage = await client.GetAsync(apiPath);
            if (responseMessage.IsSuccessStatusCode)
            {
                bankObjectArray = await responseMessage.Content.ReadAsAsync<PrivatBankObj[]>();
            }
            return bankObjectArray;
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
