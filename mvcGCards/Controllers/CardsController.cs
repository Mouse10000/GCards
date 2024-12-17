using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcGCards.Data;
using mvcGCards.Models;

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Card.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,Name,DateOfCreate,Description,Image")] CardViewModel cardView)
        {
            if (ModelState.IsValid)
            {
                Card card = new Card {
                    Name = cardView.Name,
                    DateOfCreate = cardView.DateOfCreate,
                    Description = cardView.Description
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
            CardViewModel cardView = new CardViewModel
            {
                Id = card.Id,
                Name = card.Name,
                DateOfCreate = card.DateOfCreate,
                Description = card.Description
            };
            return View(cardView);
        }

        // POST: Cards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,DateOfCreate,Description,Image")] CardViewModel cardView)
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
                    DateOfCreate = cardView.DateOfCreate,
                    Description = cardView.Description
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
