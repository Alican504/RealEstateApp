using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Service;
using RealEstateApp.Models;
using RealEstateApp.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Web.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly AppDbContext _context; // Veritabanı bağlantısını ekledik

        // Constructor: Hem servisi hem de veritabanı context'ini içeri alıyoruz
        public PropertyController(IPropertyService propertyService, AppDbContext context)
        {
            _propertyService = propertyService;
            _context = context;
        }

        // Listeleme ve Arama (Giriş Kontrollü)
        public IActionResult Index(string keyword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // KÜÇÜK AMA HAYAT KURTARAN DOKUNUŞ: 
            // Eğer arama kelimesi boş değilse, başındaki ve sonundaki boşlukları temizle
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();
            }

            var properties = _propertyService.Search(userId.Value, keyword);
            ViewBag.Keyword = keyword;
            return View(properties);
        }

        // Yeni İlan Sayfası (Create - GET)
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // Yeni İlanı Kaydetme (Create - POST)
        [HttpPost]
        public async Task<IActionResult> Create(Property property, List<IFormFile> imageFiles)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            property.UserId = userId.Value;
            property.Images = new List<PropertyImage>();

            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var file in imageFiles)
                {
                    if (file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            property.Images.Add(new PropertyImage
                            {
                                ImageData = memoryStream.ToArray(),
                                ContentType = file.ContentType
                            });
                        }
                    }
                }
            }

            _propertyService.Add(property);

            // BAŞARI BİLDİRİMİ EKLENDİ
            TempData["Success"] = "Yeni ilan başarıyla portföye eklendi!";

            return RedirectToAction("Index");
        }

        // Silme İşlemi (Delete)
        public IActionResult Delete(int id)
        {
            _propertyService.Delete(id);

            // BAŞARI BİLDİRİMİ EKLENDİ
            TempData["Success"] = "İlan sistemden kalıcı olarak silindi.";

            return RedirectToAction("Index");
        }

        // İlan Detay Sayfası
        public IActionResult Details(int id)
        {
            var property = _propertyService.GetById(id);
            if (property == null) return NotFound();
            return View(property);
        }

        // Güncelleme Sayfası (Update - GET)
        [HttpGet]
        public IActionResult Update(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // İlanı fotoğraflarıyla birlikte getiriyoruz
            var property = _context.Properties
                                   .Include(p => p.Images)
                                   .FirstOrDefault(p => p.Id == id);

            if (property == null || property.UserId != userId) return NotFound();

            return View(property);
        }

        // Güncelleme İşlemi (Update - POST)
        [HttpPost]
        public async Task<IActionResult> Update(Property property, List<IFormFile> newImages, List<int> deletedImageIds)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // 1. Mevcut ilanı ve fotoğraflarını veritabanından çekiyoruz
            var existingProperty = _context.Properties
                                           .Include(p => p.Images)
                                           .FirstOrDefault(p => p.Id == property.Id);

            if (existingProperty == null || existingProperty.UserId != userId) return NotFound();

            // 2. Metin alanlarını güncelliyoruz
            existingProperty.Title = property.Title;
            existingProperty.Type = property.Type;
            existingProperty.Price = property.Price;
            existingProperty.Location = property.Location;
            existingProperty.Status = property.Status;
            existingProperty.Description = property.Description;
            existingProperty.SquareMeters = property.SquareMeters;
            existingProperty.RoomCount = property.RoomCount;
            existingProperty.PropertyType = property.PropertyType;

            // 3. SİLME: İşaretlenen fotoğrafları veritabanından kaldırıyoruz
            if (deletedImageIds != null && deletedImageIds.Count > 0)
            {
                var imagesToDelete = existingProperty.Images
                                                     .Where(img => deletedImageIds.Contains(img.Id))
                                                     .ToList();
                _context.PropertyImages.RemoveRange(imagesToDelete);
            }

            // 4. EKLEME: Yeni seçilen çoklu fotoğrafları BLOB olarak ekliyoruz
            if (newImages != null && newImages.Count > 0)
            {
                foreach (var file in newImages)
                {
                    if (file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            existingProperty.Images.Add(new PropertyImage
                            {
                                ImageData = memoryStream.ToArray(),
                                ContentType = file.ContentType
                            });
                        }
                    }
                }
            }

            // 5. Değişiklikleri kaydediyoruz
            _context.SaveChanges();

            // BAŞARI BİLDİRİMİ EKLENDİ
            TempData["Success"] = "İlan bilgileri başarıyla güncellendi!";

            return RedirectToAction("Index");
        }

        // BAŞKA DANIŞMANIN PORTFÖYÜNÜ GÖRME (READ-ONLY)
        public IActionResult AgentPortfolio(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Account");

            // İlgili danışmanı bul
            var agent = _context.Users.FirstOrDefault(u => u.Id == id);
            if (agent == null) return NotFound();

            ViewBag.AgentName = agent.FullName;

            // Sadece o danışmana ait olan ilanları (resimleriyle birlikte) çek
            var properties = _context.Properties
                                     .Include(p => p.Images)
                                     .Where(p => p.UserId == id)
                                     .OrderByDescending(p => p.Id)
                                     .ToList();

            return View(properties);
        }
    }
}