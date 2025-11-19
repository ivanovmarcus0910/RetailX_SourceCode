using DataAccessObjectRetailX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjectRetailX.Models;

namespace RepositoriesRetailX
{
    public class RequestRepository : IRequestRepository
    {
        private readonly RequestDAO _dao;

        public RequestRepository(RequestDAO dao)
        {
            _dao = dao;
        }

        public Request GetRequestById(int requestId)
        {
            return _dao.GetRequestById(requestId);
        }

        public void CreateJoinRequest(int userId, int tenantId)
        {
            if (IsRequestPending(userId, tenantId))
                return;

            var request = new Request
            {
                UserId = userId,
                TenantId = tenantId
            };
            _dao.Add(request);
        }

        public void DeleteRequest(int requestId)
        {
            _dao.Delete(requestId);
        }

        public bool IsRequestPending(int userId, int tenantId)
        {
            return _dao.Exists(userId, tenantId);
        }

        public List<Request> GetTenantPendingRequests(int tenantId)
        {
            return _dao.GetPendingByTenant(tenantId);
        }
    }
}
