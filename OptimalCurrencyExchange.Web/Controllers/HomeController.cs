using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OptimalCurrencyExchange.Web.BLL;
using OptimalCurrencyExchange.Web.DAL;
using OptimalCurrencyExchange.Web.Models;
using OptimalCurrencyExchange.Web.Models.ViewModels;

namespace OptimalCurrencyExchange.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ExchangeService exchangeService;
        private readonly DataBaseService dataBaseService;

        public HomeController(ILogger<HomeController> logger, ExchangeService exchangeService, DataBaseService dataBaseService)
        {
            _logger = logger;
            this.exchangeService = exchangeService;
            this.dataBaseService = dataBaseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: ExchangeRates
        public async Task<IActionResult> ExchangeRatesAsync()
        {
            await exchangeService.CheckDataRelevanceAsync();
            var exchangeDbContext = dataBaseService.GetExchangeRates();
            return View(exchangeDbContext);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExchangesAsync([Bind("CurrencyFrom,CountFrom,CurrencyTo")] ExchangeRequest exchangeRequest)
        {
            if (exchangeRequest.CurrencyFrom == exchangeRequest.CurrencyTo)
                return View("Index");

            await exchangeService.CheckDataRelevanceAsync();
            var bestExchanges = exchangeService.FindBestExchanges(exchangeRequest);
            return View(bestExchanges);
        }
    }
}
