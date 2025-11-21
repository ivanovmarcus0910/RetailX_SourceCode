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
            cus.IsActive = false; 
        }
    }

    public void Save()
    {
        _context.SaveChanges();
    }
   


    public (List<Customer>, int) GetCustomers(string name, string phone, string status, int pageIndex, int pageSize)
    {
        var query = _context.Customers.AsQueryable();


        if (!string.IsNullOrEmpty(status))
        {
            if (status == "active") query = query.Where(c => c.IsActive);
            else if (status == "inactive") query = query.Where(c => !c.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            string n = name.ToLower().Trim();
            query = query.Where(c => c.CustomerName.ToLower().Contains(n));
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            string p = phone.Trim();
            query = query.Where(c => c.Phone.Contains(p));
        }

        int totalCount = query.Count();

        var list = query.OrderByDescending(c => c.CustomerId)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

        return (list, totalCount);
    }
}
