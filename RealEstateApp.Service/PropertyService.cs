using RealEstateApp.DAL;
using RealEstateApp.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RealEstateApp.Service
{
    public class PropertyService : IPropertyService
    {
        private readonly AppDbContext _context;

        public PropertyService(AppDbContext context)
        {
            _context = context;
        }

        public List<Property> GetAll()
        {
            return _context.Properties.ToList();
        }

        public List<Property> Search(int userId, string keyword)
        {
            var userProperties = _context.Properties
                                         .Include(p => p.Images)
                                         .Where(p => p.UserId == userId)
                                         .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                userProperties = userProperties.Where(p => p.Title.Contains(keyword) ||
                                                           p.Location.Contains(keyword) ||
                                                           p.Type.Contains(keyword));
            }

            return userProperties.ToList();
        }

        public Property GetById(int id)
        {
            return _context.Properties
                           .Include(p => p.Images)
                           .FirstOrDefault(p => p.Id == id);
        }

        public void Add(Property property)
        {
            _context.Properties.Add(property);
            _context.SaveChanges();
        }

        public void Update(Property property)
        {
            _context.Properties.Update(property);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var property = _context.Properties.Find(id);
            if (property != null)
            {
                _context.Properties.Remove(property);
                _context.SaveChanges();
            }
        }
    }
}