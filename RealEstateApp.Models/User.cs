using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // Soru işaretleri (?) çok önemli. Eski kayıtlarda bu alanlar boş olsa da sistemin çalışmasını sağlar.
        public byte[]? ProfilePhoto { get; set; }
        public string? ProfilePhotoType { get; set; }

        // Bire-Çok (1-N) İlişki: Bir kullanıcının BİRDEN FAZLA ilanı olabilir.
        public List<Property> Properties { get; set; }
    }
}