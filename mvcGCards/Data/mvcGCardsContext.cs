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
        public DbSet<Card> Card { get; set; }
        public DbSet<UserCard> UserCard { get; set; }
        public DbSet<Trade> Trade { get; set; }
        public DbSet<CardSender> CardSender { get; set; }
        public DbSet<CardRecipient> CardRecipient { get; set; }
    }
}
