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
            // ⚠️ Tạm thời so sánh plain text cho dễ test
            // nếu đại ca đã hash thì thay logic check ở đây
            return user.PasswordHash == password;
        }
    }
}
