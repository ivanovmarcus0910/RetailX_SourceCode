using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessObject;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDAO _dao;

        public ProductRepository(Tenant0Context context)
        {
            _dao = new ProductDAO(context);
        }

        public List<Product> GetAll() => _dao.GetAllProducts();

        public Product? GetById(int id) => _dao.GetProductById(id);

        public void Insert(Product product) => _dao.AddProduct(product);

        public void Update(Product product) => _dao.UpdateProduct(product);

        public void Delete(int id) => _dao.DeleteProduct(id);
    }
}
