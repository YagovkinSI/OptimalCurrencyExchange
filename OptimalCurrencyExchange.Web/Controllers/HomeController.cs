using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OptimalCurrencyExchange.Web.BLL;
using OptimalCurrencyExchange.Web.DAL;
using OptimalCurrencyExchange.Web.Models;
using OptimalCurrencyExchange.Web.Models.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

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

        public IActionResult ErrorPage(string message)
        {
            ViewBag.MessageError = message;
            return View();
        }

        public async Task<IActionResult> ExchangeRatesAsync(bool forcedUpdate = false)
        {
            try
            {
                ViewBag.DataRelevance = await exchangeService.CheckDataRelevanceAsync(forcedUpdate);
                var exchangeDbContext = dataBaseService.GetExchangeRates();
                return View(exchangeDbContext);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(ErrorPage), ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExchangesAsync([Bind("CurrencyFrom,CountFrom,CurrencyTo")] ExchangeRequest exchangeRequest)
        {
            if (exchangeRequest.CurrencyFrom == exchangeRequest.CurrencyTo)
                return View("Index");

            try
            {
                ViewBag.DataRelevance = await exchangeService.CheckDataRelevanceAsync();
                var bestExchanges = exchangeService.FindBestExchanges(exchangeRequest);
                return View(bestExchanges);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(ErrorPage), ex.Message);
            }
        }
    }
}
