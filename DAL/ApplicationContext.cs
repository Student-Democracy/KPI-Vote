using DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Faculty> Faculties { get; set; }

        public DbSet<Flow> Flows { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Appeal> Appeals { get; set; }

        public DbSet<Ban> Bans { get; set; }

        public DbSet<Voting> Votings { get; set; }

        public DbSet<Vote> Votes { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Vote>()
            .HasKey(vote => new { vote.VotingId, vote.UserId });
        }
    }
}
