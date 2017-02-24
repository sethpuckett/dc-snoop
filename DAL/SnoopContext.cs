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
    }
}
