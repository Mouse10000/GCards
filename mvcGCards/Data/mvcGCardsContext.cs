using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvcGCards.Models;

namespace mvcGCards.Data
{
    public class mvcGCardsContext : IdentityDbContext
    {
        public mvcGCardsContext(DbContextOptions<mvcGCardsContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Lot> Lot { get; set; }
        public DbSet<Card> Card { get; set; }
        public DbSet<Card> CardEditModel { get; set; }
    }
}
