using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace DataAccessObject
{
    public class CategoryDAO
    {
        private readonly Tenant0Context _context;

        public CategoryDAO(Tenant0Context context)
        {
            _context = context;
        }

        // Lấy tất cả category
        public List<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        // Lấy category theo ID
        public Category GetById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        // Thêm mới
        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        // Cập nhật
        public void Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        // Xóa
        public void Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }

        // Kiểm tra tồn tại
        public bool Exists(int id)
        {
            return _context.Categories.Any(c => c.CategoryId == id);
        }



    }
}
