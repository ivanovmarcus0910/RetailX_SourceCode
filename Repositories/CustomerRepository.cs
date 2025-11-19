using BusinessObject.Models;
using DataAccessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly Tenant0Context _context;
        private readonly CustomerDAO _dao;

        public CustomerRepository(Tenant0Context context)
        {
            _context = context;
            _dao = new CustomerDAO(context);
        }

        public List<Customer> GetAll() => _dao.GetAll();

        public Customer? GetById(int id) => _dao.GetById(id);

        public void Insert(Customer customer)
        {
            _dao.Add(customer);
            _context.SaveChanges();
        }

        public void Update(Customer customer)
        {
            _dao.Update(customer);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            _dao.Delete(id);
            _context.SaveChanges();
        }
    }
}
