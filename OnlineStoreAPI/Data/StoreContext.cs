using Microsoft.EntityFrameworkCore;
using OnlineStoreAPI.Models;
using System.Collections.Generic;

namespace OnlineStoreAPI.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
