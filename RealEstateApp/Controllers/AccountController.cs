using Microsoft.AspNetCore.Mvc;
using RealEstateApp.DAL;
using RealEstateApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace RealEstateApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserSession", user.FullName);
                HttpContext.Session.SetInt32("UserId", user.Id);

                TempData["Success"] = $"Sisteme hoş geldin, {user.FullName}!";

                return RedirectToAction("Index", "Property");
            }

            ViewBag.Error = "Hatalı kullanıcı adı veya şifre!";
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                ViewBag.Error = "Bu kullanıcı adı zaten alınmış!";
                return View();
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            var userProperties = _context.Properties.Where(p => p.UserId == userId).ToList();

            ViewBag.TotalProperties = userProperties.Count;
            ViewBag.ActiveProperties = userProperties.Count(p => p.Status == "Yayında");
            ViewBag.SoldProperties = userProperties.Count(p => p.Status == "Satıldı" || p.Status == "Kiralandı");

            ViewBag.RecentProperties = _context.Properties
                                               .Include(p => p.Images)
                                               .Where(p => p.UserId == userId)
                                               .OrderByDescending(p => p.Id)
                                               .Take(3)
                                               .ToList();

            return View(user);
        }

        [HttpGet]
        public IActionResult SearchAgent(string keyword)
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login");

            var agents = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                agents = agents.Where(u => u.FullName.Contains(keyword) || u.Username.Contains(keyword));
            }

            ViewBag.Keyword = keyword;

            return View(agents.ToList());
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(User updatedUser, IFormFile? profileImage)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var existingUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (existingUser == null) return NotFound();

            if (existingUser.Username != updatedUser.Username)
            {
                var isUsernameTaken = _context.Users.Any(u => u.Username == updatedUser.Username && u.Id != userId);
                if (isUsernameTaken)
                {
                    TempData["Error"] = "Bu kullanıcı adı başka bir danışman tarafından kullanılmaktadır!";
                    return RedirectToAction("EditProfile");
                }
            }

            existingUser.FullName = updatedUser.FullName;
            existingUser.Username = updatedUser.Username;

            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                existingUser.Password = updatedUser.Password;
            }

            if (profileImage != null && profileImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await profileImage.CopyToAsync(memoryStream);
                    existingUser.ProfilePhoto = memoryStream.ToArray();
                    existingUser.ProfilePhotoType = profileImage.ContentType;
                }
            }

            _context.SaveChanges();

            TempData["Success"] = "Hesap bilgileriniz başarıyla güncellendi!";

            return RedirectToAction("Profile");
        }
    }
}