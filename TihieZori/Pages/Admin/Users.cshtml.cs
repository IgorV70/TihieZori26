using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TihieZori.Data;
using TihieZori.Models;

namespace TihieZori.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UsersModel(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<UserViewModel> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserViewModel
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email ?? "",
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList()
                });
            }
        }

        public class UserViewModel
        {
            public string Id { get; set; } = "";
            public string FullName { get; set; } = "";
            public string Email { get; set; } = "";
            public bool IsActive { get; set; }
            public DateTime? CreatedAt { get; set; }
            public List<string> Roles { get; set; } = new();
        }
    }
}