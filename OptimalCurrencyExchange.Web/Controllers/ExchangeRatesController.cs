using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OptimalCurrencyExchange.Web.BLL;
using OptimalCurrencyExchange.Web.Models;
using OptimalCurrencyExchange.Web.Models.ModelsDB;

namespace OptimalCurrencyExchange.Web.Controllers
{
    public class ExchangeRatesController : Controller
    {
        private readonly ExchangeDbContext _context;

        public ExchangeRatesController(ExchangeDbContext context)
        {
            _context = context;
        }

        // GET: ExchangeRates
        public async Task<IActionResult> Index()
        {
            await ExchangeHelper.CheckDataRelevanceAsync(_context);
            var exchangeDbContext = _context.ExchangeRates.Include(e => e.Bank);
            return View(await exchangeDbContext.ToListAsync());
        }

        // GET: ExchangeRates/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exchangeRate = await _context.ExchangeRates
                .Include(e => e.Bank)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exchangeRate == null)
            {
                return NotFound();
            }

            return View(exchangeRate);
        }

        // GET: ExchangeRates/Create
        public IActionResult Create()
        {
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Id");
            return View();
        }

        // POST: ExchangeRates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BankId,CurrencySale,CurrencyBuy,Rate")] ExchangeRate exchangeRate)
        {
            if (ModelState.IsValid)
            {
                exchangeRate.Id = Guid.NewGuid();
                _context.Add(exchangeRate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Id", exchangeRate.BankId);
            return View(exchangeRate);
        }

        // GET: ExchangeRates/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exchangeRate = await _context.ExchangeRates.FindAsync(id);
            if (exchangeRate == null)
            {
                return NotFound();
            }
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Id", exchangeRate.BankId);
            return View(exchangeRate);
        }

        // POST: ExchangeRates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,BankId,CurrencySale,CurrencyBuy,Rate")] ExchangeRate exchangeRate)
        {
            if (id != exchangeRate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exchangeRate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExchangeRateExists(exchangeRate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Id", exchangeRate.BankId);
            return View(exchangeRate);
        }

        // GET: ExchangeRates/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exchangeRate = await _context.ExchangeRates
                .Include(e => e.Bank)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exchangeRate == null)
            {
                return NotFound();
            }

            return View(exchangeRate);
        }

        // POST: ExchangeRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var exchangeRate = await _context.ExchangeRates.FindAsync(id);
            _context.ExchangeRates.Remove(exchangeRate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExchangeRateExists(Guid id)
        {
            return _context.ExchangeRates.Any(e => e.Id == id);
        }
    }
}
