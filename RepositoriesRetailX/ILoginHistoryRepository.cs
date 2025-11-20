using BusinessObjectRetailX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesRetailX
{
    public interface ILoginHistoryRepository
    {
        List<UserLoginHistory> GetLoginHistories();
        List<UserLoginHistory> GetLoginHistoriesByTenantId(int id);

        UserLoginHistory GetLoginHistoryById(int id);
        bool AddLoginHistory(UserLoginHistory history);

    }
}
