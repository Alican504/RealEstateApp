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

        public byte[]? ProfilePhoto { get; set; }
        public string? ProfilePhotoType { get; set; }

        public List<Property> Properties { get; set; }
    }
}