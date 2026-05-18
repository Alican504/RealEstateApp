using RealEstateApp.Models;
using System.Collections.Generic;

namespace RealEstateApp.Service
{
    public interface IPropertyService
    {
        List<Property> GetAll();
        List<Property> Search(int userId, string keyword); Property GetById(int id);
        void Add(Property property);
        void Update(Property property);
        void Delete(int id);
    }
}