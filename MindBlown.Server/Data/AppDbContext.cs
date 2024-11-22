using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Models;

namespace MindBlown.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public required DbSet<Mnemonic> Mnemonics { get; set; }

        public required DbSet<LastWrongAnswerRecord> Record { get; set; }

        public required DbSet<User> ActiveUserSession { get; set; }
    }
}