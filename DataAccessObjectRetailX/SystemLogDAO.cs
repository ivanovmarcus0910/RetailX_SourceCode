using BusinessObjectRetailX.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjectRetailX
{
    public class SystemLogDAO
    {
        private readonly RetailXContext _context;
        public SystemLogDAO(RetailXContext context)
        {
            _context = context;
        }
        public List<SystemLog> GetLogList()
        {
            return _context.SystemLogs.Include(l => l.User).ToList();
        }
        public SystemLog GetLog(int id)
        {
            return _context.SystemLogs
        .Include(l => l.User)
        .FirstOrDefault(l => l.Id == id);
        }
        public bool AddLog(SystemLog log)
        {
            try
            {
                _context.SystemLogs.Add(log);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
      
    }
}
