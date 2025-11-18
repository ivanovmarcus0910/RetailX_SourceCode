using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IOrderRepository
    {
        List<Order> GetAll();
        Order? GetById(int id);
        void Insert(Order order);
        void Delete(int id);

        Order CreateOrder(Order order, List<OrderDetail> details);
        List<Order> GetOrdersByStaff(int staffId);
    }

}
