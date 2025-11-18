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

        public User GetUserByEmail(string email)
        {
            
                return _context.Users.FirstOrDefault(u => u.Email == email);
            
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

        

    }
}
