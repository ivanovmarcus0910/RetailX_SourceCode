using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICustomerRepository
    {
        List<Customer> GetAll();
        Customer? GetById(int id);
        void Insert(Customer customer);
        void Update(Customer customer);
        void Delete(int id);
        Customer? GetByPhone(string phone);
        Customer? GetByEmail(string email);
        // Trả về Tuple (Danh sách khách, Tổng số khách)
        (List<Customer>, int) GetCustomers(string name, string phone, string status, int page, int size);
    }
}

