using GitGenius.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GitGenius.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult PendingUsers()
        {
            var pendingUsers = _context.Users
                .Where(u => u.Status == GitGenius.Models.UserStatus.Pending)
                .ToList();

            return View(pendingUsers);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.Status = GitGenius.Models.UserStatus.Approved;
                _context.SaveChanges();
            }

            return RedirectToAction("PendingUsers");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("PendingUsers");
        }
    }
}