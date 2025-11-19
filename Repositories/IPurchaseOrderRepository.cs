using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repositories
{
    public interface IPurchaseOrderRepository
    {
        List<PurchaseOrder> GetAll();
        PurchaseOrder GetById(int id);
        void Add(PurchaseOrder order);
        void Update(PurchaseOrder order);
        void Delete(int id);
    }
}
