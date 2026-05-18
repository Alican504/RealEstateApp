using Microsoft.AspNetCore.Mvc;
using RealEstateApp.DAL;
using RealEstateApp.Models;
using Microsoft.AspNetCore.Http; // Session işlemleri için gerekli
using Microsoft.EntityFrameworkCore;

namespace RealEstateApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        // Veritabanını içeri alıyoruz (Dependency Injection)
        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GİRİŞ YAP (GET)
        [HttpGet]
        public IActionResult Login() => View();

        // 2. GİRİŞ YAP (POST)
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Veritabanında bu kullanıcı adı ve şifreyle eşleşen biri var mı?
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // GİRİŞ BAŞARILI! 
                // Artık sadece ismini değil, kullanıcının ID'sini de aklımızda (Session) tutmalıyız!
                HttpContext.Session.SetString("UserSession", user.FullName);
                HttpContext.Session.SetInt32("UserId", user.Id);

                // BAŞARI BİLDİRİMİ EKLENDİ (Karşılama Mesajı)
                TempData["Success"] = $"Sisteme hoş geldin, {user.FullName}!";

                return RedirectToAction("Index", "Property");
            }

            ViewBag.Error = "Hatalı kullanıcı adı veya şifre!";
            return View();
        }

        // 3. KAYIT OL (GET)
        [HttpGet]
        public IActionResult Register() => View();

        // 4. KAYIT OL (POST)
        [HttpPost]
        public IActionResult Register(User user)
        {
            // Aynı kullanıcı adından başka var mı diye kontrol et (Kayıt tekrarını önler)
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                ViewBag.Error = "Bu kullanıcı adı zaten alınmış!";
                return View();
            }

            // Yeni kullanıcıyı veritabanına ekle
            _context.Users.Add(user);
            _context.SaveChanges();

            // Kayıt başarılıysa giriş sayfasına yönlendir
            return RedirectToAction("Login");
        }

        // 5. ÇIKIŞ YAP
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // 6. PROFİL SAYFASI (GET)
        [HttpGet]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            // 1. Kullanıcıya ait tüm ilanları çekelim
            var userProperties = _context.Properties.Where(p => p.UserId == userId).ToList();

            // 2. İstatistikleri hesaplayıp ViewBag ile sayfaya gönderelim
            ViewBag.TotalProperties = userProperties.Count;
            ViewBag.ActiveProperties = userProperties.Count(p => p.Status == "Yayında");
            ViewBag.SoldProperties = userProperties.Count(p => p.Status == "Satıldı" || p.Status == "Kiralandı");

            // 3. Son eklenen 3 ilanı (görselleriyle birlikte) "Son İlanlar" vitrini için alalım
            ViewBag.RecentProperties = _context.Properties
                                               .Include(p => p.Images)
                                               .Where(p => p.UserId == userId)
                                               .OrderByDescending(p => p.Id)
                                               .Take(3)
                                               .ToList();

            return View(user);
        }

        // 7. DANIŞMAN ARAMA SAYFASI
        [HttpGet]
        public IActionResult SearchAgent(string keyword)
        {
            // Güvenlik: Giriş yapmamış biri danışmanları arayamaz
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login");

            var agents = _context.Users.AsQueryable();

            // Eğer arama kutusuna bir isim yazıldıysa filtrele
            if (!string.IsNullOrEmpty(keyword))
            {
                // Büyük/küçük harf duyarlılığını azaltmak için Contains kullanıyoruz
                agents = agents.Where(u => u.FullName.Contains(keyword) || u.Username.Contains(keyword));
            }

            ViewBag.Keyword = keyword;
            // Kendi adımızı listede görmemek istersen bu satırı açabilirsin:
            // var currentUserId = HttpContext.Session.GetInt32("UserId");
            // agents = agents.Where(u => u.Id != currentUserId);

            return View(agents.ToList());
        }

        // 8. PROFİL DÜZENLEME SAYFASI (GET)
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return View(user);
        }

        // 9. PROFİL GÜNCELLEME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> EditProfile(User updatedUser, IFormFile? profileImage)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var existingUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (existingUser == null) return NotFound();

            // -- YENİ EKLENEN BENZERSİZ KULLANICI ADI KONTROLÜ --
            if (existingUser.Username != updatedUser.Username)
            {
                var isUsernameTaken = _context.Users.Any(u => u.Username == updatedUser.Username && u.Id != userId);
                if (isUsernameTaken)
                {
                    TempData["Error"] = "Bu kullanıcı adı başka bir danışman tarafından kullanılmaktadır!";
                    return RedirectToAction("EditProfile");
                }
            }
            // ---------------------------------------------------

            // Temel Bilgileri Güncelle
            existingUser.FullName = updatedUser.FullName;
            existingUser.Username = updatedUser.Username;

            // Eğer şifre alanı boş değilse şifreyi de güncelle
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                existingUser.Password = updatedUser.Password;
            }

            // Fotoğraf İşlemi
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

            // BAŞARI BİLDİRİMİ EKLENDİ
            TempData["Success"] = "Hesap bilgileriniz başarıyla güncellendi!";

            return RedirectToAction("Profile");
        }
    }
}