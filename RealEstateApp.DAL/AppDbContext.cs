using Microsoft.EntityFrameworkCore;
using RealEstateApp.Models; // Modellerimizin olduğu katmanı kullanıyoruz

namespace RealEstateApp.DAL
{
    public class AppDbContext : DbContext
    {
        // Constructor (Yapıcı Metot) - Veritabanı ayarlarını dışarıdan (Program.cs'den) almamızı sağlar
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Veritabanında oluşacak tablolarımız
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }

        public DbSet<PropertyImage> PropertyImages { get; set; }
    }
}