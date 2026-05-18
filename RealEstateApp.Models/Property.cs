namespace RealEstateApp.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; } // Örn: Keçiören'de 3+1 Kiralık
        public string Type { get; set; } // Satılık, Kiralık
        public string Location { get; set; }
        public decimal Price { get; set; }

        public string RoomCount { get; set; } // Örn: "3+1", "1+1", "Villa (5+2)"
        public string PropertyType { get; set; } // Örn: "Daire", "Villa", "Rezidans", "Ofis"
        public int SquareMeters { get; set; }

        public string Status { get; set; } // Yayında, Satıldı vb.
        public string? Description { get; set; }

        public List<PropertyImage>? Images { get; set; }

        // Yabancı Anahtar (Foreign Key) - Bu ilanı hangi danışman ekledi?
        public int UserId { get; set; }

        // Navigation Property - İlan üzerinden danışmanın bilgilerine (adına vb.) ulaşmak için
        public User User { get; set; }
    }
}