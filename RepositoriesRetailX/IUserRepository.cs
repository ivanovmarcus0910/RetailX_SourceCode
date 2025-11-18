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
        User GetUserByEmail(string email);
        bool VerifyUser(string email, string password);
        bool SignUpUser(string email, string password, string fullname);
        bool UpdateUser(User user);
    }
}
