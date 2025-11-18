using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessObject;

namespace Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
      
        private readonly SupplierDAO _supplierDAO;

        public SupplierRepository(SupplierDAO supplierDAO)
        {
            _supplierDAO = supplierDAO;
        }

        public IEnumerable<Supplier> GetAllSuppliers()
        {
            return _supplierDAO.GetAll();
        }

        public Supplier GetSupplierById(int id)
        {
            return _supplierDAO.GetById(id);
        }

        public void AddSupplier(Supplier supplier)
        {
            _supplierDAO.Add(supplier);
        }

        public void UpdateSupplier(Supplier supplier)
        {
            _supplierDAO.Update(supplier);
        }

        public void DeleteSupplier(int id)
        {
            _supplierDAO.Delete(id);
        }
    }
}
