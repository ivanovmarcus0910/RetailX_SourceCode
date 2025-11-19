using BusinessObjectRetailX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesRetailX
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User GetUserByEmail(string email);
        User GetUserById(int userId);
        bool VerifyUser(string email, string password);
        bool SignUpUser(string email, string password, string fullname);
        bool UpdateUser(User user);
    }
}
