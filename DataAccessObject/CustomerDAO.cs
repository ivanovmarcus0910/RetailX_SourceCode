using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class CustomerDAO
    {
        private readonly Tenant0Context _context;

        public CustomerDAO(Tenant0Context context)
        {
            _context = context;
        }
        public List<Customer> GetAll()
        {
            return _context.Customers
                .Where(c => c.IsActive == true)
                .ToList();
        }
        public Customer? GetById(int id)
        {
            return _context.Customers
                .FirstOrDefault(c => c.CustomerId == id);
        }

        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
        }

        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);
        }
        public void Delete(int id)
        {
            var cus = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (cus != null)
            {
                _context.Customers.Remove(cus);
            }
        }
    }
}
