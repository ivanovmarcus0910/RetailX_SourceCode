using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BusinessObjectRetailX.Models;

namespace DataAccessObjectRetailX
{
    public class RequestDAO
    {
        private readonly RetailXContext _context;

        public RequestDAO(RetailXContext context)
        {
            _context = context;
        }

        public Request GetRequestById(int requestId)
        {
            return _context.Requests.FirstOrDefault(r => r.Id == requestId);
        }

        public void Add(Request request)
        {
            _context.Requests.Add(request);
            _context.SaveChanges();
        }

        public void Delete(int requestId)
        {
            var request = _context.Requests.Find(requestId);
            if (request != null)
            {
                _context.Requests.Remove(request);
                _context.SaveChanges();
            }
        }

        public List<Request> GetPendingByTenant(int tenantId)
        {
            return _context.Requests
                .Where(r => r.TenantId == tenantId)
                .Include(r => r.User)
                .ToList();
        }
        public List<Request> GetRequestsByUserId(int userId)
        {
            return _context.Requests
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                .Include(r => r.Tenant)
                .OrderByDescending(r => r.Id)
                .ToList();
        }

        public bool Exists(int userId, int tenantId)
        {
            return _context.Requests
                .Any(r => r.UserId == userId && r.TenantId == tenantId);
        }
    }
}
