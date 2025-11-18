using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

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
            return _context.Categories.Where(p => p.IsActive == true).ToList();
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
                category.IsActive = false;
                _context.Entry(category).State = EntityState.Modified;

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
