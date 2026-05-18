namespace RealEstateApp.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }

        public string RoomCount { get; set; }
        public string PropertyType { get; set; }
        public int SquareMeters { get; set; }

        public string Status { get; set; }
        public string? Description { get; set; }

        public List<PropertyImage>? Images { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}