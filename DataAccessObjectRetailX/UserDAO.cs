using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjectRetailX;
using BusinessObjectRetailX.Models;
namespace DataAccessObjectRetailX
{
    public class UserDAO
    {
        private readonly RetailXContext _context;
        public UserDAO(RetailXContext context)
        {
            _context = context;
        }

        public List<User> GetAllUser()
        {
            return _context.Users.ToList();
        }

        public User GetUserByEmail(string email)
        {
            
                return _context.Users.FirstOrDefault(u => u.Email == email);
            
        }

        public User GetUserById(int id)
        {
           
                return _context.Users.FirstOrDefault(u => u.Id == id);

        }
        public bool AddUser(User user)
        {
           
                try
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            
        }
        public bool DeleteUser(int id) {
          
                var user = _context.Users.Find(id);
                try
                {
                    user.IsActive = false;
                    _context.SaveChanges();
                    return true;
                } catch (Exception)
                {
                    return false;
                }
            
        }
        public bool UpdateUser(User input)
        {
            var existing = _context.Users.FirstOrDefault(u => u.Id == input.Id);
            if (existing == null) return false;

            existing.FullName = string.IsNullOrWhiteSpace(input.FullName)
                ? existing.FullName
                : input.FullName;

            existing.Email = string.IsNullOrWhiteSpace(input.Email)
                ? existing.Email
                : input.Email;

            existing.Phone = string.IsNullOrWhiteSpace(input.Phone)
                ? existing.Phone
                : input.Phone;

            existing.GlobalRole = string.IsNullOrWhiteSpace(input.GlobalRole)
                ? existing.GlobalRole
                : input.GlobalRole;

            // 🔹 TenantId: nếu null thì giữ cũ
            existing.TenantId = input.TenantId ?? existing.TenantId;

            // 🔹 IsActive: chỉ update khi có giá trị (true/false)
            if (input.IsActive.HasValue)
                existing.IsActive = input.IsActive;

            // 🔹 PasswordHash: thường tách form riêng, nhưng nếu cho update:
            if (!string.IsNullOrWhiteSpace(input.PasswordHash))
                existing.PasswordHash = input.PasswordHash;

            // ❌ Không đụng CreatedDate, Id
            _context.SaveChanges();
            return true;
        }
        public int QuantityUserActive()
        {
            return _context.Users.Count(u => u.IsActive==true);

        }
        public int QuantityUser()
        {
            return _context.Users.Count();

        }


    }
}
