using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TihieZori.Data;
using TihieZori.Models;

namespace TihieZori.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int AdminUsers { get; set; }
        public int NewUsersLast7Days { get; set; }
        public List<UserInfo> RecentUsers { get; set; } = new();

        public async Task OnGetAsync()
        {
            var users = await _context.Users.ToListAsync();

            TotalUsers = users.Count;
            ActiveUsers = users.Count(u => u.IsActive);
            NewUsersLast7Days = users.Count(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-7));

            // Подсчет админов (упрощенно - нужно через роли)
            AdminUsers = 1; // TODO: подсчет через UserManager

            RecentUsers = users
                .OrderByDescending(u => u.CreatedAt)
                .Take(10)
                .Select(u => new UserInfo
                {
                    FullName = $"{u.FirstName} {u.LastName}".Trim(),
                    Email = u.Email ?? "",
                    CreatedAt = u.CreatedAt
                })
                .ToList();
        }

        public class UserInfo
        {
            public string FullName { get; set; } = "";
            public string Email { get; set; } = "";
            public DateTime? CreatedAt { get; set; }
        }
    }
}