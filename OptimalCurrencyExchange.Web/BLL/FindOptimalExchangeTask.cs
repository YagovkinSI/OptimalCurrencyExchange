﻿using OptimalCurrencyExchange.Web.Models.Enums;
using OptimalCurrencyExchange.Web.Models.ModelsBL;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using OptimalCurrencyExchange.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimalCurrencyExchange.BLL
{
    public class FindAllExchangesTask
    {
        private readonly ExchangeRequest exchangeRequest;
        private readonly List<ExchangeRate> exchangeDbContext;

        public List<Exchange> Result = new List<Exchange>();

        public FindAllExchangesTask(ExchangeRequest exchangeRequest, List<ExchangeRate> exchangeDbContext)
        {
            this.exchangeRequest = exchangeRequest;
            this.exchangeDbContext = exchangeDbContext;
        }

        public void Exucute()
        {
            var currencyChain = new List<List<enCurrency>>();
            FillCurrencyChain(currencyChain, exchangeRequest.CurrencyTo, new List<enCurrency> { exchangeRequest.CurrencyFrom });
            var exchanges = CalcChains(currencyChain);
            var sortedExchanges = exchanges.OrderBy(e => e.CountTo).Reverse().ToList();
            Result = sortedExchanges;
        }

        private void FillCurrencyChain(List<List<enCurrency>> variants, enCurrency toCurrency, List<enCurrency> currentSteps)
        {
            foreach (var currency in Enum.GetValues(typeof(enCurrency)))
            {
                if ((enCurrency)currency == enCurrency.Unknow || currentSteps.Contains((enCurrency)currency))
                    continue;
                else if (toCurrency == (enCurrency)currency)
                {
                    var list = new List<enCurrency>(currentSteps.Count + 1);
                    list.AddRange(currentSteps);
                    list.Add(toCurrency);
                    variants.Add(list);
                }
                else
                {
                    var newSteps = new List<enCurrency>(currentSteps.Count + 1);
                    newSteps.AddRange(currentSteps);
                    newSteps.Add((enCurrency)currency);
                    FillCurrencyChain(variants, toCurrency, newSteps);
                }
            }
        }

        private List<Exchange> CalcChains(List<List<enCurrency>> currencyChain)
        {
            var exchanges = new List<Exchange>();
            foreach (var chain in currencyChain)
            {
                var exchangesForChain = new List<Exchange>();
                FillExchangesForChain(ref exchangesForChain, chain, new List<ExchangeStep>());
                exchanges.AddRange(exchangesForChain);
            }
            return exchanges;
        }

        private void FillExchangesForChain(ref List<Exchange> exchangesForChain, List<enCurrency> chain, List<ExchangeStep> steps)
        {
            var stepNumber = steps.Count + 1;
            var rates = exchangeDbContext
                    .Where(e => e.CurrencySale == chain[stepNumber - 1] && e.CurrencyBuy == chain[stepNumber])
                    .Where(e => e.Rate > 0);
            foreach (var rate in rates)
            {
                var countFrom = steps.LastOrDefault()?.CountTo ?? exchangeRequest.CountFrom;
                var countTo = Math.Round(countFrom / rate.Rate, 2, MidpointRounding.ToNegativeInfinity);
                var step = new ExchangeStep
                {
                    CurrencyFrom = chain[stepNumber - 1],
                    CurrencyTo = chain[stepNumber],
                    CountFrom = countFrom,
                    CountTo = countTo,
                    BankId = rate.BankId,
                    Bank = rate.Bank
                };
                var newSteps = new List<ExchangeStep>(steps.Count + 1);
                newSteps.AddRange(steps);
                newSteps.Add(step);
                if (stepNumber == chain.Count - 1)
                {
                    var exchange = new Exchange()
                    {
                        CurrencyFrom = exchangeRequest.CurrencyFrom,
                        CurrencyTo = exchangeRequest.CurrencyTo,
                        CountFrom = exchangeRequest.CountFrom,
                        CountTo = countTo,
                        DateTime = DateTime.Now,
                        ExchangeSteps = newSteps
                    };
                    exchangesForChain.Add(exchange);
                }
                else
                {
                    FillExchangesForChain(ref exchangesForChain, chain, newSteps);
                }
            }
        }
    }
}
