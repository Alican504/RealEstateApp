using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Models
{
    public class PropertyImage
    {
        [Key]
        public int Id { get; set; }

        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}