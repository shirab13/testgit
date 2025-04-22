using Microsoft.AspNetCore.Mvc;
using GitGenius.Models;
using GitGenius.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace GitGenius.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AccountController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "אימייל זה כבר קיים במערכת.");
                return View(model);
            }

            if (_context.Users.Any(u => u.IdNumber == model.IdNumber))
            {
                ModelState.AddModelError("IdNumber", "תעודת זהות זו כבר קיימת במערכת.");
                return View(model);
            }

            if (model.ApprovalFile == null)
            {
                ModelState.AddModelError("ApprovalFile", "יש לצרף קובץ אישור לימודים.");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            // שמירת הקובץ
            string? filePath = null;
            if (model.ApprovalFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ApprovalFile.FileName;
                filePath = Path.Combine("uploads", uniqueFileName);

                var fullPath = Path.Combine(_environment.WebRootPath, filePath);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.ApprovalFile.CopyToAsync(stream);
                }
            }

            // יצירת המשתמש לשמירה במסד
            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                IdNumber = model.IdNumber,
                Password = model.Password, // בהמשך נשתמש ב־Hash
                Role = model.Role,
                Status = UserStatus.Pending,
                IsAdmin = false,
                ApprovalFilePath = filePath
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "נרשמת בהצלחה! החשבון ממתין לאישור אדמין.";
            return RedirectToAction("Register");
        }


        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "אימייל או סיסמה לא נכונים.");
                return View(model);
            }

            if (user.Status != UserStatus.Approved)
            {
                ModelState.AddModelError("", "החשבון שלך עדיין לא אושר על ידי אדמין.");
                return View(model);
            }

            //  שמירת המשתמש ב־Session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("IsAdmin", user.IsAdmin ? "true" : "false");
            HttpContext.Session.SetString("Role", user.Role.ToString());




            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult ManageUsers()
        {
            // בדיקה אם המשתמש המחובר הוא אדמין
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var pendingUsers = _context.Users
                .Where(u => u.Status == UserStatus.Pending)
                .Select(u => new PendingUserViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    IdNumber = u.IdNumber,
                    Role = u.Role.ToString(),
                    ApprovalFilePath = u.ApprovalFilePath
                }).ToList();

            return View(pendingUsers);
        }

        [HttpPost]
        public IActionResult ApproveUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.Status = UserStatus.Approved;
                _context.SaveChanges();
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public IActionResult RejectUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageUsers");
        }

        public IActionResult Logout()
        {
            // ניקוי כל ה־Session
            HttpContext.Session.Clear();

            // חזרה לדף הבית
            return RedirectToAction("Index", "Home");
        }

    }
}
