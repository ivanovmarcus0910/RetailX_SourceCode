using BusinessObject.Models;
using DataAccessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly LogDAO _logDAO;

        public LogRepository(LogDAO logDAO)
        {
            _logDAO = logDAO;
        }

        public void LogCreate(string message, int staffId)
        {
            SaveLogToDAO(message, staffId, 1);
        }

        public void LogUpdate(string message, int staffId)
        {
            SaveLogToDAO(message, staffId, 2);
        }

        public void LogWarning(string message, int staffId)
        {
            SaveLogToDAO(message, staffId, 3);
        }

        private void SaveLogToDAO(string message, int staffId, byte level)
        {
            var log = new Log
            {
                Decription = message,
                StaffId = staffId,
                LogLevel = level,
                CreateDate = DateTime.Now
            };
            _logDAO.AddLog(log);
        }
    }
}
