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
        Order CreateOrder(Order order, List<OrderDetail> details);

        Order? GetOrderById(int id);

        List<Order> GetOrdersByStaff(int staffId);
    }

}
