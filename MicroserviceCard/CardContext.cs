
using MicroserviceCard.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MicroserviceCard
{
    public class CardContext : DbContext
    {
        public CardContext(DbContextOptions<CardContext> options)
            : base(options)
        {
        }
        public DbSet<Card> Card { get; set; }
    }

}