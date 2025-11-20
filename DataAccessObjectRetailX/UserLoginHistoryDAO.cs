using BusinessObjectRetailX.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjectRetailX
{
    public class UserLoginHistoryDAO
    {
        private readonly RetailXContext _context;
        public UserLoginHistoryDAO(RetailXContext context)
        {
            _context = context;
        }
        public List<UserLoginHistory> GetAll()
        {
            return _context.UserLoginHistories.Include(l => l.User).ToList();
        }
        public List<UserLoginHistory> GetLoginHistoryByTenantId(int tenantId)
        {
            return _context.UserLoginHistories
                .Where(l => l.TenantId == tenantId)
                .OrderByDescending(l => l.LoginTime) // nếu có cột này, không có thì bỏ
                .ToList();
        }
        public UserLoginHistory GetLoginHistoryById(int id)
        {
            return _context.UserLoginHistories
        .Include(l => l.User)
        .FirstOrDefault(l => l.Id == id);
        }
         
        public bool AddLog(UserLoginHistory x)
        {
            try
            {
                _context.UserLoginHistories.Add(x);
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
