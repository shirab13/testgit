using Microsoft.AspNetCore.Mvc;
using GitGenius.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace GitGenius.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound();

            return View(user);
        }
    }
}
