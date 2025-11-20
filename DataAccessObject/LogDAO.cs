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

        public List<Log> GetLogsByFilter(DateTime? fromDate, DateTime? toDate, int? logLevel, int count)
        {
            var query = _context.Logs
                .Include(l => l.Staff)
                .AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(l => l.CreateDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var endOfDay = toDate.Value.AddDays(1);
                query = query.Where(l => l.CreateDate < endOfDay);
            }

            if (logLevel.HasValue && logLevel.Value > 0)
            {
                query = query.Where(l => l.LogLevel == logLevel.Value);
            }

            return query.OrderByDescending(l => l.CreateDate)
                        .Take(count)
                        .ToList();
        }
    }
}
