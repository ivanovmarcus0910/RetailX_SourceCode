using BusinessObjectRetailX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesRetailX
{
    public interface ITenantRepository
    {
        List<Tenant> GetAllTenant();
        Tenant? GetTenantById(int id);
        Tenant? GetTenantByOwnerEmail(string email);

        bool AddTenant(Tenant tenant);
        bool DeleteTenant(int id);
        string BuildTenantConnectionString(Tenant tenant);

        bool UpdateTenant(Tenant tenant);

    }
}
