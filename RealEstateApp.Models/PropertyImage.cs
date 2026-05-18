using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Models
{
    public class PropertyImage
    {
        [Key]
        public int Id { get; set; }

        // Fotoğrafın byte dizisi ve tipi
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }

        // Bire-Çok İlişki: Bu fotoğraf hangi ilana ait?
        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}