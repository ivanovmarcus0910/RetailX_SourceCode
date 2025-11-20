using BusinessObjectRetailX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesRetailX
{
    public interface ISystemLogRepository
    {
        List<SystemLog> GetAll();
        SystemLog GetById(int id);
        bool AddLog(SystemLog log);
    }
}
