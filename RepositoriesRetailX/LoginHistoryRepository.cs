using BusinessObjectRetailX.Models;
using DataAccessObjectRetailX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesRetailX
{
    public class LoginHistoryRepository : ILoginHistoryRepository
    {
        private readonly UserLoginHistoryDAO loginHistoryDAO;
        public LoginHistoryRepository(UserLoginHistoryDAO loginHistoryDAO)
        {
            this.loginHistoryDAO = loginHistoryDAO;
        }
        public bool AddLoginHistory(UserLoginHistory history)
        {
            return loginHistoryDAO.AddLog(history);
        }

        public List<UserLoginHistory> GetLoginHistories()
        {
            return loginHistoryDAO.GetAll();
        }

        public List<UserLoginHistory> GetLoginHistoriesByTenantId(int id)
        {
            return loginHistoryDAO.GetLoginHistoryByTenantId(id);
        }

        public UserLoginHistory GetLoginHistoryById(int id)
        {
            return loginHistoryDAO.GetLoginHistoryById(id);
        }
    }
}
