using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class LogDAO
    {
        private readonly Tenant0Context _context;

        public LogDAO(Tenant0Context context)
        {
            _context = context;
        }

        public void AddLog(Log log)
        {
            _context.Logs.Add(log);
            _context.SaveChanges();
        }

        public List<Log> GetRecentLogs(int count)
        {
            return _context.Logs.Include(l => l.Staff)
                           .OrderByDescending(l => l.CreateDate)
                           .Take(count)
                           .ToList();
        }
    }
}
