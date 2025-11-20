using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
namespace Repositories
{
    public interface ILogRepository
    {
        void LogCreate(string message, int staffId);

        void LogUpdate(string message, int staffId);

        void LogWarning(string message, int staffId);

        List<Log> GetRecentLogs(int count);
        List<Log> GetLogsByFilter(DateTime? fromDate, DateTime? toDate, int? logLevel, int count);
    }
}
