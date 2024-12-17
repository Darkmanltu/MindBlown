using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Models;

namespace MindBlown.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Mnemonic> Mnemonics { get; set; }

        public DbSet<LastWrongAnswerRecord> Record { get; set; }

        public DbSet<User> ActiveUserSession { get; set; }

        public DbSet<UserMnemonicIDs> UserWithMnemonicsIDs { get; set; }

        public required DbSet<AnswerSession> AnswerSessions { get; set; }
        public required DbSet<AnsweredMnemonic> AnsweredMnemonics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
        base.OnModelCreating(modelBuilder);

        // Configure primary key for AnswerSession
        modelBuilder.Entity<AnswerSession>()
            .HasKey(ams => ams.AnswerSessionId);

        // Configure primary key for AnsweredMnemonic
        modelBuilder.Entity<AnsweredMnemonic>()
            .HasKey(am => am.AnsweredMnemonicId);

        // Define the one-to-many relationship between AnswerSession and AnsweredMnemonic
        modelBuilder.Entity<AnsweredMnemonic>()
            .HasOne(am => am.AnswerSession)
            .WithMany(ans => ans.AnsweredMnemonics)
            .HasForeignKey(am => am.AnswerSessionId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete on session deletion
    }

    }
}