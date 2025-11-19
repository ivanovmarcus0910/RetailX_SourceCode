using BusinessObjectRetailX.Models;
using DataAccessObjectRetailX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesRetailX
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _userDao;

        public UserRepository(UserDAO userDao)
        {
            _userDao = userDao;
        }
        public User GetUserByEmail(string email)
        {
           
            return _userDao.GetUserByEmail(email);
        }
        public bool VerifyUser(string email, string password)
        {
            var user = _userDao.GetUserByEmail(email);
            if (user == null)
            {
                return false;
            }
            return user.PasswordHash == password;
        }
        public bool SignUpUser(string email, string password, string fullname)
        {
            var newUser = new User
            {
                Email = email,
                PasswordHash = password, 
                FullName = fullname,
                IsActive = true,
                CreatedDate = DateTime.Now,
                GlobalRole = "User"
            };
            return _userDao.AddUser(newUser);
        }
        public bool UpdateUser(User user)
        {
            return _userDao.UpdateUser(user);
        }
    }
}
