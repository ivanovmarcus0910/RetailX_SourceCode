using BusinessObjectRetailX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjectRetailX
{
    public class TenantDAO
    {
        private readonly RetailXContext _context;
        public TenantDAO(RetailXContext context)
        {
            _context = context;
        }
        public List<Tenant> GetAllTenant()
        {
            return _context.Tenants.ToList();
        }
        public List<Tenant> GetAllTenantActive()
        {
            return _context.Tenants.Where(t => t.IsActive == true).ToList();
        }
        public Tenant GetTenantById(int tenantId)
        {
            return _context.Tenants.Find(tenantId);
        }
        public Tenant GetTenantByOwnerEmail(string email)
        {
            var tenants = _context.Tenants.ToList();
            return tenants.FirstOrDefault(t => t.OwnerEmail == email);
        }
       

        public bool DeleteTenant(int tenantId)
        {
            var tenant = _context.Tenants.Find(tenantId);
            try
            {
                tenant.IsActive = false;
                _context.SaveChanges();
                return true;
            } catch (Exception)
            {
                return false;
            }
                
            
        }
        public bool AddTenant(Tenant tenant)
        {
            try
            {
                _context.Tenants.Add(tenant);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public int QuantityTenantActive()
        {
            return _context.Tenants.Count(u => u.IsActive == true);

        }
        public int QuantityTenant()
        {
            return _context.Tenants.Count();

        }

        public bool UpdateTenant(Tenant tenant)
        {
            try
            {
                _context.Tenants.Update(tenant);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
