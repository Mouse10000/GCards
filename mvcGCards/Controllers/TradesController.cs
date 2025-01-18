using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcGCards.Data;
using mvcGCards.Models;

namespace mvcGCards.Controllers
{
    [Authorize]
    public class TradesController : Controller
    {
        private readonly mvcGCardsContext _context;

        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        public TradesController(
            Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager,
            mvcGCardsContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Trades
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trade.ToListAsync());
        }

        // GET: Trades/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trade = await _context.Trade
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trade == null)
            {
                return NotFound();
            }

            return View(trade);
        }

        // GET: Trades/Create
        public IActionResult CreateUser()
        {
            var items0 = _userManager.Users.ToList();
            List<string> users = [];
            for (int counter = 0; counter < items0.Count; counter++)
            {
                users.Add(items0[counter].UserName);
            }
            TradeUserView tradeUserCreateView = new()
            {
                UserSender = "",
                UserRecipient = "",
                State = "Create",
                Users = users
            };
            return View(tradeUserCreateView);
        }

        // POST: Trades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("Id,UserSender,UserRecipient,State")] Trade trade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateSenderCards),
                    new { tradeID = trade.Id, userSender = trade.UserSender });
            }
            return View(trade);
        }

        // GET: Trades/CreateSenderCards
        public IActionResult CreateSenderCards(string userSender, int tradeId)
        {

            var trade = _context.Trade
                .FirstOrDefault(m => m.Id == tradeId);
            IQueryable<UserCard> userCards0 = _context.UserCard; //отбираем значения нашего пользователя
            userCards0 = userCards0.Where(p => p.UserName!.Contains(trade.UserSender));

            var userCardsList0 = userCards0.ToList();
            var items0 = _context.Card.ToList();
            var items = from a in items0
                        from b in userCardsList0
                        where a.Id == b.CardId
                        select a;

            List<TradeCard> userCardsList = new List<TradeCard>();
            foreach (Card item in items)
            {
                TradeCard tradeSenderCardView = new TradeCard
                {
                    IsChecked = false,
                    Card = item
                };
                userCardsList.Add(tradeSenderCardView);
            }
            TradeSenderView tradeSenderView = new()
            {
                TradeId = tradeId,
                TradeCards = userCardsList
            };
            return View(tradeSenderView);
        }

        // POST: Trades/CreateSenderCards
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSenderCards(TradeSenderView tradeSenderView)
        {
            var trade = await _context.Trade
                .FirstOrDefaultAsync(m => m.Id == tradeSenderView.TradeId);
            for (int i = 0; i < tradeSenderView.TradeCards.Count; i++)
            {
                if (tradeSenderView.TradeCards[i].IsChecked)
                {
                    CardSender cardSender = new()
                    {
                        TradeId = tradeSenderView.TradeId,
                        CardId = tradeSenderView.TradeCards[i].Card.Id
                    };
                    _context.Add(cardSender);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(CreateRecipientCards),
            new { tradeID = trade.Id, userSender = trade.UserSender });

        }

        // GET: Trades/CreateRecipientCards
        public IActionResult CreateRecipientCards(string userSender, int tradeId)
        {

            var trade = _context.Trade
                .FirstOrDefault(m => m.Id == tradeId);
            IQueryable<UserCard> userCards0 = _context.UserCard; //отбираем значения нашего пользователя
            userCards0 = userCards0.Where(p => p.UserName!.Contains(trade.UserRecipient));
            var userCardsList0 = userCards0.ToList();
            var items0 = _context.Card.ToList();
            var items = from a in items0
                        from b in userCardsList0
                        where a.Id == b.CardId
                        select a;

            List<TradeCard> userCardsList = new List<TradeCard>();
            foreach (Card item in items)
            {
                TradeCard tradeSenderCardView = new TradeCard
                {
                    IsChecked = false,
                    Card = item
                };
                userCardsList.Add(tradeSenderCardView);
            }
            TradeSenderView tradeSenderView = new()
            {
                TradeId = tradeId,
                TradeCards = userCardsList
            };
            return View(tradeSenderView);
        }

        // POST: Trades/CreateRecipientCards
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRecipientCards(
            [Bind("TradeId,TradeCards")] TradeSenderView tradeSenderView)
        {

            var trade = await _context.Trade
                .FirstOrDefaultAsync(m => m.Id == tradeSenderView.TradeId);
            for (int i = 0; i < tradeSenderView.TradeCards.Count; i++)
            {
                if (tradeSenderView.TradeCards[i].IsChecked)
                {
                    CardRecipient cardRecipient = new()
                    {
                        TradeId = tradeSenderView.TradeId,
                        CardId = tradeSenderView.TradeCards[i].Card.Id
                    };
                    _context.Add(cardRecipient);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Trades/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trade = await _context.Trade.FindAsync(id);
            if (trade == null)
            {
                return NotFound();
            }
            return View(trade);
        }

        // POST: Trades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,UserSender,UserRecipient,State")] Trade trade)
        {
            if (id != trade.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TradeExists(trade.Id))
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
            return View(trade);
        }

        // GET: Trades/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trade = await _context.Trade
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trade == null)
            {
                return NotFound();
            }

            return View(trade);
        }

        // POST: Trades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var trade = await _context.Trade.FindAsync(id);
            if (trade != null)
            {
                _context.Trade.Remove(trade);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TradeExists(long id)
        {
            return _context.Trade.Any(e => e.Id == id);
        }
    }
}
