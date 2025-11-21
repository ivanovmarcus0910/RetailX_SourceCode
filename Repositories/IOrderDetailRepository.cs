using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IOrderDetailRepository
    {

        OrderDetail? GetById(int id);

  
        List<OrderDetail> GetByOrder(int orderId);


        void Add(OrderDetail detail);


        void Update(OrderDetail detail);

        
        void Delete(int detailId);
    }
}
