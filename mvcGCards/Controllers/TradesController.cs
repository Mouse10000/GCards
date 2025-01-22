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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using mvcGCards.Data;
using mvcGCards.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var trade = _context.Trade
                .FirstOrDefault(m => m.Id == id);
            var cards = _context.Card.ToList();

            IQueryable<CardSender> cardsSender = _context.CardSender; //отбираем значения нашего пользователя
            cardsSender = cardsSender.Where(p => p.TradeId!.Equals(trade.Id));
            var cardsSenderList0 = cardsSender.ToList();
            var itemsSender = from a in cards
                         from b in cardsSenderList0
                         where a.Id == b.CardId
                         select a;
            var cardsSenderList = itemsSender.ToList();

            IQueryable<CardRecipient> cardsRecipient = _context.CardRecipient; //отбираем значения нашего пользователя
            cardsRecipient = cardsRecipient.Where(p => p.TradeId!.Equals(trade.Id));
            var cardsRecipientList0 = cardsRecipient.ToList();
            var itemsRecipient = from a in cards
                                 from b in cardsRecipientList0
                                 where a.Id == b.CardId
                                 select a;
            var cardsRecipientList = itemsRecipient.ToList();

            if (trade == null)
            {
                return NotFound();
            }
            TradeDetailView tradeDetail = new()
            {
                Trade = trade,
                CardSender = cardsSenderList,
                CardRecipient = cardsRecipientList
            };
            return View(tradeDetail);
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
                return RedirectToAction(nameof(CreateRecipientCards),
                    new { tradeID = trade.Id });
            }
            return View(trade);
        }

        // GET: Trades/CreateRecipientCards
        public IActionResult CreateRecipientCards(long tradeId)
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

            trade.State = "AddRecipientCards";
            _context.Update(trade);
            _context.SaveChanges();
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
                    CardRecipient cardRecipientDb = new()
                    {
                        TradeId = tradeSenderView.TradeId,
                        CardId = tradeSenderView.TradeCards[i].Card.Id
                    };
                    _context.Add(cardRecipientDb);

                    trade.State = "AddSenderCards";
                    _context.Update(trade);

                    await _context.SaveChangesAsync();
                }
            }
            if (trade == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(CreateSenderCards),
                new { tradeID = trade.Id });
        }


        // GET: Trades/CreateSenderCards
        public IActionResult CreateSenderCards(long tradeId)
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

                    trade.State = "NotSended";
                    _context.Update(trade);

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Edit), new { id = tradeSenderView.TradeId });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirm(TradeDetailView tradeDetailView)
        {

            var trade = await _context.Trade.FindAsync(tradeDetailView.Trade.Id);
            if (trade == null)
            {
                return View(tradeDetailView);
            }

            trade.State = "Sended";
            _context.Update(trade);

            await _context.SaveChangesAsync();

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
            switch (trade.State)
            {
                case "Create":
                    return RedirectToAction(nameof(CreateSenderCards),
                        new { tradeID = trade.Id });
                case "AddSenderCards":
                    return RedirectToAction(nameof(CreateSenderCards),
                        new { tradeID = trade.Id });
                case "AddRecipientCards":
                    return RedirectToAction(nameof(CreateRecipientCards),
                        new { tradeID = trade.Id });
                case "NotSended":
                    return RedirectToAction(nameof(Details),
                        new { id = trade.Id });
                case "Sended":
                    return View();
                default:
                    return NotFound();
            }
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

        // GET: Trades/ConfirmSend/5 = GET: Trades/Details/5 
        // POST: Trades/ConfirmSend/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmSend(TradeDetailView tradeDetailView)
        {

            var trade = await _context.Trade.FindAsync(tradeDetailView.Trade.Id);
            if (trade == null)
            {
                return View(tradeDetailView);
            }

            trade.State = "Sended";
            _context.Update(trade);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Trades/AcceptTrade/5 = GET: Trades/Details/5 
        // POST: Trades/AcceptTrade/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsseptTrade(TradeDetailView tradeDetailView)
        {

            var trade = await _context.Trade.FindAsync(tradeDetailView.Trade.Id);
            if (trade == null)
            {
                return NotFound();
            }

            IQueryable<UserCard> userCardsSender = _context.UserCard
                .Where(p => p.UserName!.Equals(trade.UserSender));
            var userCardsSenderList = userCardsSender.ToList();
            Console.WriteLine("userCardsSenderList = ", userCardsSenderList.Count());
            Console.WriteLine(userCardsSenderList.Count());

            var cardsSenderList0 = _context.CardSender
                .Where(p => p.TradeId!.Equals(trade.Id)).ToList();
            Console.WriteLine("cardsSenderList0 = ", cardsSenderList0.Count());
            Console.WriteLine(cardsSenderList0.Count());

            var itemsSender = from a in userCardsSenderList
                              from b in cardsSenderList0
                              where a.CardId == b.CardId
                              select a;
            var cardsSenderList = itemsSender.ToList();
            Console.WriteLine("cardsSenderList = ", cardsSenderList.Count());
            Console.WriteLine(cardsSenderList.Count());

            var userCardsRecipientList = _context.UserCard
                .Where(p => p.UserName!.Equals(trade.UserRecipient)).ToList();
            Console.WriteLine("userCardsRecipientList = ", userCardsRecipientList.Count());
            Console.WriteLine(userCardsRecipientList.Count());
            var cardsRecipientList0 = _context.CardRecipient
                .Where(p => p.TradeId!.Equals(trade.Id)).ToList();
            Console.WriteLine("cardsRecipientList0 = ", cardsRecipientList0.Count());
            Console.WriteLine(cardsRecipientList0.Count());

            var itemsRecipient = from a in userCardsRecipientList
                                 from b in cardsRecipientList0
                                 where a.CardId == b.CardId
                                 select a;
            var cardsRecipientList = itemsRecipient.ToList();
            Console.WriteLine("cardsRecipientList = ", cardsRecipientList.Count());
            Console.WriteLine(cardsRecipientList.Count());

            var allCards = new List<UserCard>();
            allCards.AddRange(cardsSenderList);
            allCards.AddRange(cardsRecipientList);
            Console.WriteLine("allCards = ", allCards.Count());
            Console.WriteLine(allCards.Count());

            foreach (UserCard userCard in allCards)
            {
                Console.WriteLine(userCard.Id);
                Console.WriteLine(userCard.UserName);
                Console.WriteLine(userCard.CountDublicate);
                if (userCard.CountDublicate > 0)
                {
                    userCard.CountDublicate -= 1;
                    _context.Update(userCard);
                }
                else
                {
                    _context.Remove(userCard);
                }
                string newUser = "";
                if (userCard.UserName == trade.UserSender) newUser = trade.UserRecipient;
                else newUser = trade.UserSender;

                var userCardOld = await _context.UserCard.Where(p => p.CardId == userCard.CardId)
                    .FirstOrDefaultAsync(m => m.UserName == newUser);
                if (userCardOld != null)
                {
                    userCardOld.CountDublicate += 1;
                    _context.Update(userCardOld);
                }
                else
                {
                    UserCard newUserCard = new()
                    {
                        UserName = newUser,
                        CardId = userCard.CardId,
                        CountDublicate = 0,
                    };
                    _context.Add(newUserCard);
                }
                await _context.SaveChangesAsync();
            }
            trade.State = "Accepted";
            _context.Update(trade);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Trades/AcceptTrade/5 = GET: Trades/Details/5 
        // POST: Trades/AcceptTrade/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisasseptTrade(TradeDetailView tradeDetailView)
        {

            var trade = await _context.Trade.FindAsync(tradeDetailView.Trade.Id);
            if (trade == null)
            {
                return NotFound();
            }

            trade.State = "Disaccepted";

            _context.Update(trade);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
            if(trade.State == "Sended")
            {
                return RedirectToAction(nameof(Edit), new { id = trade.Id });
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
