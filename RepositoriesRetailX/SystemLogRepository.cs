using BusinessObjectRetailX.Models;
using DataAccessObjectRetailX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesRetailX
{
    public class SystemLogRepository : ISystemLogRepository
    {
        private readonly SystemLogDAO systemLogDAO;
        public SystemLogRepository(SystemLogDAO systemLogDAO)
        {
            this.systemLogDAO = systemLogDAO;
        }

        public bool AddLog(SystemLog log)
        {
            return systemLogDAO.AddLog(log);
        }

        public List<SystemLog> GetAll()
        {
            return systemLogDAO.GetLogList();
        }

        public SystemLog GetById(int id)
        {
            return systemLogDAO.GetLog(id);
        }
    }
}
