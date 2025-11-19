using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessObject;

namespace Repositories
{
    public class PurchaseRepository : IPurchaseOrderRepository
    {
        private readonly PurchaseOrderDAO _dao;

        public PurchaseRepository(PurchaseOrderDAO dao)
        {
            _dao = dao;
        }

        public List<PurchaseOrder> GetAll() => _dao.GetAll();
        public PurchaseOrder GetById(int id) => _dao.GetById(id);
        public void Add(PurchaseOrder order) => _dao.Add(order);
        public void Update(PurchaseOrder order) => _dao.Update(order);
        public void Delete(int id) => _dao.Delete(id);
    }
}
