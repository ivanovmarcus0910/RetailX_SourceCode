using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessObject;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _dao;

        public CategoryRepository(CategoryDAO dao)
        {
            _dao = dao;
        }

        public List<Category> GetAll() => _dao.GetAll();
        public Category GetById(int id) => _dao.GetById(id);
        public void Add(Category category) => _dao.Add(category);
        public void Update(Category category) => _dao.Update(category);
        public void Delete(int id) => _dao.Delete(id);
    }
}
