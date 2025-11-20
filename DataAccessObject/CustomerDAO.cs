using BusinessObject.Models;
using DataAccessObject;

public class CustomerDAO
{
    private readonly Tenant0Context _context;

    public CustomerDAO(Tenant0Context context)
    {
        _context = context;
    }

    public List<Customer> GetAll()
    {
        return _context.Customers.ToList();
    }

    public Customer? GetById(int id)
    {
        return _context.Customers.FirstOrDefault(c => c.CustomerId == id);
    }

    public Customer? GetByPhone(string phone)
    {
        return _context.Customers.FirstOrDefault(c => c.Phone == phone);
    }

    public Customer? GetByEmail(string email)
    {
        return _context.Customers.FirstOrDefault(c => c.Email == email);
    }

    public void Add(Customer customer)
    {
        _context.Customers.Add(customer);
    }

    public void Update(Customer incoming)
    {
        var dbCus = _context.Customers.FirstOrDefault(c => c.CustomerId == incoming.CustomerId);
        if (dbCus == null) return;

        dbCus.CustomerName = incoming.CustomerName;
        dbCus.Phone = incoming.Phone;
        dbCus.Email = incoming.Email;
        dbCus.Address = incoming.Address;
    }


    public void Delete(int id)
    {
        var cus = GetById(id);
        if (cus != null)
        {
            cus.IsActive = false; // Soft delete
        }
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
