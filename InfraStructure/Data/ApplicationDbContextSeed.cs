using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfraStructure.Data
{
    public class ApplicationDbContextSeed
    {
        private ApplicationDbContext _context;

        public ApplicationDbContextSeed(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            _context.Database.Migrate();
            _context.Database.EnsureCreated();

            var roleStore = new RoleStore<IdentityRole>(_context);
            await roleStore.CreateAsync(new IdentityRole { Name = "Admin", NormalizedName = "Admin" });

            var user = new ApplicationUser
            {
                UserName = "mgmg",
                NormalizedUserName = "MGMG",
                Email = "mgmg@gmail.com",
                NormalizedEmail = "MGMG@GMAIL.COM",
                EmailConfirmed = false,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            if (!_context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "P@ssw0rd");
                user.PasswordHash = hashed;
                var userStore = new UserStore<ApplicationUser>(_context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "Admin");
            }
            await _context.SaveChangesAsync();
        }
    }
}