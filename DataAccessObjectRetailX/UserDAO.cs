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
            using (var context = new RetailXContext())
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }
        public bool AddUser(User user)
        {
            using (var context = new RetailXContext())
            {
                try
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
        }
        public bool DeleteUser(int id) {
            using (var context = new RetailXContext())
            {
                var user = context.Users.Find(id);
                try
                {
                    user.IsActive = false;
                    context.SaveChanges();
                    return true;
                } catch (Exception)
                {
                    return false;
                }
            }
        }

        

    }
}
