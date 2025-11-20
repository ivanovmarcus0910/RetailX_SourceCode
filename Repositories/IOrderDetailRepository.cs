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
        // Lấy 1 dòng chi tiết theo ID
        OrderDetail? GetById(int id);

        // Lấy tất cả OrderDetail của 1 Order
        List<OrderDetail> GetByOrder(int orderId);

        // Thêm 1 dòng OrderDetail
        void Add(OrderDetail detail);

        // Update 1 dòng OrderDetail (chỉ sửa Quantity hoặc ProductId)
        void Update(OrderDetail detail);

        // Xóa 1 dòng OrderDetail
        void Delete(int detailId);
    }
}
