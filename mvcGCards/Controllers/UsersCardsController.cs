using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcGCards.Data;
using mvcGCards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Authentication;
namespace mvcGCards.Controllers
{
    [Authorize]
    public class UsersCardsController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        private readonly mvcGCardsContext _context;

        public UsersCardsController(mvcGCardsContext context)
        {
            _context = context;
        }

        // GET: UserCards/user@mail.com

        public async Task<IActionResult> Index(string? userName, string name ="", string rank = "all", int page = 1,
            SortState sortOrder = SortState.NameAsc)
        {
            int pageSize = 9;
            Console.WriteLine(userName);
            IQueryable<Card> cards = _context.Card;

            IQueryable<UserCard> userCards = _context.UserCard; //отбираем значения нашего пользователя
            userCards = userCards.Where(p => p.UserName!.Contains(userName));
            Console.WriteLine($"count userCards {userCards.Count()}");
            Console.WriteLine($"userName {userName}");
            //фильтрация
            if (!string.IsNullOrEmpty(rank) && rank != "all")
            {
                cards = cards.Where(p => p.Rank!.Equals(rank));
            }
            if (!string.IsNullOrEmpty(name))
            {
                cards = cards.Where(p => p.Name!.Contains(name));
            }
            var userCardsId = userCards.ToList();

            
            // сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    cards = cards.OrderByDescending(s => s.Name);
                    break;
                case SortState.NumberAsc:
                    cards = cards.OrderBy(s => s.Number);
                    break;
                case SortState.NumberDesc:
                    cards = cards.OrderByDescending(s => s.Number);
                    break;
                case SortState.RankAsc:
                    cards = cards.OrderBy(s => s.Rank);
                    break;
                case SortState.RankDesc:
                    cards = cards.OrderByDescending(s => s.Rank);
                    break;
                default:
                    cards = cards.OrderBy(s => s.Name);
                    break;
            }
            if (userCardsId.Count() == 0)
            {
                cards = cards.Where(p => p.Rank!.Equals(""));
            }
            // пагинация
            //var items0 = await cards.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var items0 = await cards.ToListAsync();
            //смотрим что карты есть у нашего пользователя
            var items = from a in items0
                    from b in userCards
                    where a.Id == b.CardId
                    select a;
            var count = items.Count();
            Console.WriteLine($"count items {items.Count()}");
            items = items.Skip((page - 1) * pageSize).Take(pageSize);

            List<string> ranks = new List<string>() { "R", "SR", "SSR", "UR" };


            // формируем модель представления
            IndexUserViewModel viewModel = new IndexUserViewModel(
                items,
                new PageViewModel(count, page, pageSize),
                new FilterViewModel(ranks, rank, name),
                new SortViewModel(sortOrder),
                userName
            );
            return View(viewModel);
        }

        // GET: Cards/Details/5
        public async Task<IActionResult> Details(long? idCard, string userName)
        {

            if (idCard == null)
            {
                return NotFound();
            }
            var card = await _context.Card
                .FirstOrDefaultAsync(m => m.Id == idCard);
            var userCard = await _context.UserCard.Where(p => p.CardId == idCard)
                .FirstOrDefaultAsync(m => m.UserName == userName);
            if (card == null)
            {
                return NotFound();
            }
            UserCardModel userCardRemoveModel = new UserCardModel
            {
                Card = card,
                UserName = userName,
                Count = userCard.CountDublicate + 1

            };
            return View(userCardRemoveModel);
        }
        // Get UsersCards/Add/5
        [Authorize]
        public IActionResult Add(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            UserCard userCard = new UserCard
            {
                CardId = (long)id
            };

            return View(userCard);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("Id,UserName,CardId,CountDublicate")] UserCard userCard)
        {
            if (ModelState.IsValid)
            {
                var userCardDb = await _context.UserCard
                    .FirstOrDefaultAsync(m => m.CardId == userCard.CardId);
                if (userCardDb == null)
                {

                    _context.UserCard.Add(userCard);
                }
                else
                {
                    userCardDb.CountDublicate = userCard.CountDublicate + 1;
                    _context.UserCard.Update(userCardDb);
                }
                await _context.SaveChangesAsync();

                System.Console.Out.WriteLine(userCard);
                return RedirectToAction(nameof(Index), "Cards");
            }

            return View(userCard);
        }

        // GET: UsersCards/Remove/5
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Remove(long? idCard, string userName)
        {

            if (idCard == null)
            {
                return NotFound();
            }
            var card = await _context.Card
                .FirstOrDefaultAsync(m => m.Id == idCard);
            var userCard = await _context.UserCard.Where(p => p.CardId == idCard)
                .FirstOrDefaultAsync(m => m.UserName == userName);
            if (card == null)
            {
                return NotFound();
            }
            UserCardModel userCardRemoveModel = new UserCardModel
            {
                Card = card,
                UserName = userName,
                Count = userCard.CountDublicate + 1

            };
            return View(userCardRemoveModel);
        }
        // POST: UsersCards/Remove/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(Card card, string userName, int count)
        {
            IQueryable<UserCard> userCards = _context.UserCard; //отбираем значения нашего пользователя
            var userCardDb = userCards.Where(p => p.UserName!.Contains(userName))
                .Where(p => p.UserName!.Contains(userName)).ToList();
            var userCard = await _context.UserCard
                .FirstOrDefaultAsync(m => m.Id == userCardDb[0].Id);
            var countDublicate = userCard.CountDublicate - count;
            Console.WriteLine($"count D {userCard.CountDublicate}");
            if (countDublicate < 0)
            {
                _context.UserCard.Remove(userCard);
            }
            else
            {
                userCard.CountDublicate = countDublicate;
                _context.UserCard.Update(userCard);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "UsersCards",  new {userName=userName});
        }

        private bool CardExists(long id)
        {
            return _context.Card.Any(e => e.Id == id);
        }
    }

}
