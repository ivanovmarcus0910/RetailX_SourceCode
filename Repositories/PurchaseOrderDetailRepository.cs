using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessObject;

namespace Repositories
{
    public class PurchaseOrderDetailRepository : IPurchaseOrderDetailRepository
    {
        private readonly PurchaseOrderDetailDAO _dao; 
        public PurchaseOrderDetailRepository(PurchaseOrderDetailDAO dao) 
        { _dao = dao; }
        public List<PurchaseOrderDetail> GetByOrder(int orderId) => _dao.GetByOrder(orderId); 
        public void Add(PurchaseOrderDetail detail) => _dao.Add(detail);
        public void Update(PurchaseOrderDetail detail) => _dao.Update(detail); // thêm Update
        public PurchaseOrderDetail GetById(int detailId) => _dao.GetById(detailId); // thêm


        public void Delete(int id) => _dao.Delete(id);
    }
}
