using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvcGCards.Models;
using Microsoft.AspNetCore.Identity;
namespace mvcGCards.Data
{
    public class mvcGCardsContext : IdentityDbContext
    {
        public mvcGCardsContext(DbContextOptions<mvcGCardsContext> options)
            : base(options)
        {
        }
        public DbSet<Lot> Lot { get; set; }
        public DbSet<Card> Card { get; set; }
        public DbSet<UserCard> UserCard { get; set; }
    }
}
