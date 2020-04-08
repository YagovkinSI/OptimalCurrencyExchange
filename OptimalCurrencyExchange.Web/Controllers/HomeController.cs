using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OptimalCurrencyExchange.Web.BLL;
using OptimalCurrencyExchange.Web.Models;
using OptimalCurrencyExchange.Web.Models.ModelsDB;
using OptimalCurrencyExchange.Web.Models.ViewModels;

namespace OptimalCurrencyExchange.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ExchangeDbContext _context;

        public HomeController(ILogger<HomeController> logger, ExchangeDbContext context)
        {
            _logger = logger;
            _context = context;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindOptimalExchangeAsync([Bind("CurrencyFrom,CountFrom,CurrencyTo")] ExchangeRequest exchangeRequest)
        {
            await ExchangeHelper.CheckDataRelevanceAsync(_context);
            var bestExchanges = ExchangeHelper.FindBestExchanges(_context, exchangeRequest);
            return View(new Exchange());
        }
    }
}
