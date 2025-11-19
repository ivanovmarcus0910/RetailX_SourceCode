using BusinessObjectRetailX.Models;
using DataAccessObjectRetailX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesRetailX
{
    public class TenantRepository : ITenantRepository
    {
        private readonly TenantDAO tenantDAO;
        public TenantRepository(TenantDAO tenantDAO)
        {
            this.tenantDAO = tenantDAO;
        }
        public bool AddTenant(Tenant tenant)
        {
            return tenantDAO.AddTenant(tenant);
        }

        public bool DeleteTenant(int id)
        {
            return tenantDAO.DeleteTenant(id);  
        }

        public List<Tenant> GetAllTenant()
        {
            return tenantDAO.GetAllTenant();
        }

        public Tenant? GetTenantById(int id)
        {
            return tenantDAO.GetTenantById(id);
        }

        public Tenant? GetTenantByOwnerEmail(string email)
        {
            return tenantDAO.GetTenantByOwnerEmail(email);
        }
        public string BuildTenantConnectionString(Tenant tenant)
        {
            return $"Server=.;Database={tenant.DbName};Trusted_Connection=True;TrustServerCertificate=True;";
        }
        public bool UpdateTenant(Tenant tenant)
        {
            return tenantDAO.UpdateTenant(tenant);
        }

    }
}
