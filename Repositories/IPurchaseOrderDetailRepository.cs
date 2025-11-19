using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repositories
{
    public interface IPurchaseOrderDetailRepository
    {
        List<PurchaseOrderDetail> GetByOrder(int orderId);
        void Add(PurchaseOrderDetail detail);
        void Delete(int id);
    }
}
