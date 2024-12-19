using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcGCards.Data;
using mvcGCards.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace mvcGCards.Controllers
{
    [Authorize]
    public class CardsController : Controller
    {
        private readonly mvcGCardsContext _context;

        public CardsController(mvcGCardsContext context)
        {
            _context = context;
        }

        // GET: Cards
        public async Task<IActionResult> Index(string name, string rank = "all", int page = 1,
            SortState sortOrder = SortState.NameAsc)
        {
            int pageSize = 5;
            //фильтрация
            IQueryable<Card> cards = _context.Card;
            if (!string.IsNullOrEmpty(rank) && rank != "all")
            {
                cards = cards.Where(p => p.Rank!.Contains(rank));
            }
            if (!string.IsNullOrEmpty(name))
            {
                cards = cards.Where(p => p.Name!.Contains(name));
            }
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

            // пагинация
            var count = await cards.CountAsync();
            var items = await cards.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();


            List<string> ranks = new List<string>() { "R", "SR", "SSR", "UR" };


            // формируем модель представления
            IndexViewModel viewModel = new IndexViewModel(
                items,
                new PageViewModel(count, page, pageSize),
                new FilterViewModel(ranks, rank, name),
                new SortViewModel(sortOrder)
            );
            return View(viewModel);
        }

        // GET: Cards/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Card
                .FirstOrDefaultAsync(m => m.Id == id);
            if (card == null)
            {
                return NotFound();
            }

            return View(card);
        }

        // GET: Cards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DateOfCreate,Description,Rank,Number,Image")] CardViewModel cardView)
        {
            if (ModelState.IsValid)
            {
                Card card = new Card
                {
                    Name = cardView.Name,
                    DateOfAdd = DateTime.Now.Date,
                    Description = cardView.Description,
                    Rank = cardView.Rank,
                    Number = cardView.Number
                };

                if (cardView.Image != null)
                {
                    byte[] imageData = null;
                    // считываем переданный файл в массив байтов
                    using (var binaryReader = new BinaryReader(cardView.Image.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)cardView.Image.Length);
                    }
                    // установка массива байтов
                    card.Image = imageData;
                }
                _context.Card.Add(card);
                //_context.SaveChanges();

                _context.Add(card);
                await _context.SaveChangesAsync();
                System.Console.Out.WriteLine(card);
                return RedirectToAction(nameof(Index));
            }
            return View(cardView);
        }
        // GET: Cards/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Card.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }
            Console.WriteLine(card.DateOfAdd);
            return View(card);
        }

        // POST: Cards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,DateOfAdd,Description,Rank,Number,Image")] Card card)
        {

            if (id != card.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                /*Card card = new Card
                {
                    Id = cardView.Id,
                    Name = cardView.Name,
                    Description = cardView.Description,
                    Rank = cardView.Rank,
                    Number = cardView.Number
                };
                Console.WriteLine(card.DateOfAdd);
                if (cardView.Image != null)
                {
                    byte[] imageData = null;
                    // считываем переданный файл в массив байтов
                    using (var binaryReader = new BinaryReader(cardView.Image.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)cardView.Image.Length);
                    }
                    // установка массива байтов
                    card.Image = imageData;
                }*/
                try
                {
                    _context.Update(card);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardExists(card.Id))
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
            return View(card);
        }
        // GET: Cards/EditImage/5
        public async Task<IActionResult> EditImage(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Card.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }
            Console.WriteLine(card.DateOfAdd);
            CardViewModel cardView = new CardViewModel
            {
                Id = card.Id,
                Name = card.Name,
                DateOfAdd = card.DateOfAdd,
                Description = card.Description,
                Rank = card.Rank,
                Number = card.Number
            };
            return View(cardView);
        }
        // POST: Cards/EditImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage(long id, [Bind("Id,Name,DateOfAdd,Description,Rank,Number,Image")] CardViewModel cardView)
        {

            if (id != cardView.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                Card card = new Card
                {
                    Id = cardView.Id,
                    Name = cardView.Name,
                    DateOfAdd = cardView.DateOfAdd,
                    Description = cardView.Description,
                    Rank = cardView.Rank,
                    Number = cardView.Number
                };
                Console.WriteLine(card.DateOfAdd);
                if (cardView.Image != null)
                {
                    byte[] imageData = null;
                    // считываем переданный файл в массив байтов
                    using (var binaryReader = new BinaryReader(cardView.Image.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)cardView.Image.Length);
                    }
                    // установка массива байтов
                    card.Image = imageData;
                }
                try
                {
                    _context.Update(card);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardExists(card.Id))
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
            return View(cardView);
        }
        // GET: Cards/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Card
                .FirstOrDefaultAsync(m => m.Id == id);
            if (card == null)
            {
                return NotFound();
            }

            return View(card);
        }

        // POST: Cards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var card = await _context.Card.FindAsync(id);
            if (card != null)
            {
                _context.Card.Remove(card);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CardExists(long id)
        {
            return _context.Card.Any(e => e.Id == id);
        }
    }
}
