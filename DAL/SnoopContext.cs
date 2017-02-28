using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc_snoop.Models;
using Microsoft.EntityFrameworkCore;

namespace dc_snoop.DAL
{
    public class SnoopContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Person> People { get; set; }
        
        public SnoopContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>().HasIndex(a => a.Street);
            modelBuilder.Entity<Address>().HasIndex(a => a.StreetNumber);
            modelBuilder.Entity<Address>().HasIndex(a => a.StreetName);
            modelBuilder.Entity<Address>().HasIndex(a => a.StreetQuadrant);
        }
    }
}
