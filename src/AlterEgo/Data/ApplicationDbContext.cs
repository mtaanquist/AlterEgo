using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AlterEgo.Models;

namespace AlterEgo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Define keys
            builder.Entity<Guild>()
                .HasKey(g => new { g.Name, g.Realm });

            builder.Entity<Character>()
                .HasKey(c => new { c.Name, c.Realm });

            builder.Entity<Member>()
                .HasKey(m => new { m.GuildName, m.GuildRealm, m.CharacterName, m.CharacterRealm });

            builder.Entity<News>()
                .HasKey(n => new { n.Timestamp, n.Character });

            // Define relations
            builder.Entity<Guild>()
                .HasMany(g => g.News)
                .WithOne(n => n.Guild)
                .HasForeignKey(n => new { n.GuildName, n.GuildRealm })
                .HasPrincipalKey(g => new { g.Name, g.Realm });

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Characters)
                .WithOne();

            builder.Entity<Member>()
                .HasOne(m => m.Guild)
                .WithMany(g => g.Members)
                .HasForeignKey(m => new { m.GuildName, m.GuildRealm });

            builder.Entity<Member>()
                .HasOne(m => m.Character)
                .WithOne(c => c.Member);
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Criteria> Criteria { get; set; }

        #region Forum
        public DbSet<Category> Categories { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Post> Posts { get; set; }
        #endregion
    }
}
