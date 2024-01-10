using DoodleLike.Models.entity;
using DoodleLike.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DoodleLike.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public DbSet<User> Users { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }

        public DbSet<VoteHistory> VoteHistory { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }

}
