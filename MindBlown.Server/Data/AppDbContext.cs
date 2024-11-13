using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Models;

namespace MindBlown.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Mnemonic> Mnemonics { get; set; }

        public DbSet<User> ActiveUserSession { get; set; }
    }
}