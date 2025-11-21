using BusinessObject.Models;
using DataAccessObject;
using Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerDAO _dao;

    public CustomerRepository(Tenant0Context context)
    {
        _dao = new CustomerDAO(context);
    }

    public List<Customer> GetAll() => _dao.GetAll();

    public Customer? GetById(int id) => _dao.GetById(id);

    public Customer? GetByPhone(string phone) => _dao.GetByPhone(phone);

    public Customer? GetByEmail(string email) => _dao.GetByEmail(email);

    public void Insert(Customer customer)
    {
        _dao.Add(customer);
        _dao.Save();
    }

    public void Update(Customer customer)
    {
        _dao.Update(customer);
        _dao.Save();
    }

    public void Delete(int id)
    {
        _dao.Delete(id);
        _dao.Save();
    }
    public (List<Customer>, int) GetCustomers(string name, string phone, string status, int page, int size)
    {
        return _dao.GetCustomers(name, phone, status, page, size);
    }
}
