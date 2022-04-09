using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OleMissChatbox.Data.Entities;

namespace OleMissChatbox.Data
{
    public class OleMissChatboxContext : DbContext
    {
        private readonly IConfiguration _config;
        public OleMissChatboxContext(IConfiguration config)
        {
            _config = config;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<UserChatMessage> UserChatMessages { get; set; }
        public DbSet<UserClass> UserClasses { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(_config["ConnectionStrings:OleMissChatboxContextDb"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
