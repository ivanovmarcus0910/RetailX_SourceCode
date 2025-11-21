using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjectRetailX.Models;

namespace RepositoriesRetailX
{
    public interface IRequestRepository
    {
        Request GetRequestById(int requestId);
        void CreateJoinRequest(int userId, int tenantId);
        void DeleteRequest(int requestId);
        bool IsRequestPending(int userId, int tenantId);
        List<Request> GetRequestsByUserId(int userId);
        List<Request> GetTenantPendingRequests(int tenantId);
    }
}
