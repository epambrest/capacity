using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Teams.Data.Models;

namespace Teams.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Team { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Sprint> Sprint { get; set; }
        public DbSet<Task> Task { get; set; }
        public DbSet<MemberWorkingDays> MemberWorkingDays { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Team>()
                .HasIndex(t => t.TeamName)
                .IsUnique();
        }
    }
}
