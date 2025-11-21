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
        public List<User> GetAll()
        {
            return _userDao.GetAllUser();
        }
        public User GetUserByEmail(string email)
        {
           
            return _userDao.GetUserByEmail(email);
        }

        public User GetUserById(int userId)
        {
            return _userDao.GetUserById(userId);
        }

        public bool VerifyUser(string email, string password)
        {
            var user = _userDao.GetUserByEmail(email);
            if (user == null)
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
        public bool SignUpUser(string email, string password, string fullname)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Email = email,
                PasswordHash = passwordHash, 
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
