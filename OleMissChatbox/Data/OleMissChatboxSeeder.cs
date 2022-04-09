using Microsoft.AspNetCore.Hosting;
using OleMissChatbox.Data.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OleMissChatbox.Data
{
    public class OleMissChatboxSeeder
    {
        private readonly OleMissChatboxContext _ctx;
        private readonly IWebHostEnvironment _env;
        public OleMissChatboxSeeder(OleMissChatboxContext ctx, IWebHostEnvironment env)
        {
            _ctx = ctx;
            _env = env;
        }

        public void Seed()
        {
            _ctx.Database.EnsureCreated();

            if (!_ctx.Users.Any())
            {
                // sample data
                var filePath = Path.Combine(_env.ContentRootPath, "Data/Users.json");
                var json = File.ReadAllText(filePath);
                var users = JsonSerializer.Deserialize<IEnumerable<User>>(json);

                foreach (var user in users)
                {
                    user.CreatedDate = System.DateTime.Now;
                }

                _ctx.Users.AddRange(users);


                _ctx.SaveChanges();
            }
        }
    }
}
