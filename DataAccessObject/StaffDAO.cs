using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessObject
{
    public class StaffDAO
    {
        private readonly Tenant0Context _context;

        public StaffDAO(Tenant0Context context)
        {
            _context = context;
        }

        public List<Staff> GetAll()
        {
            return _context.Staff.AsNoTracking().ToList();
        }

        public Staff GetById(int staffId)
        {
            return _context.Staff.Find(staffId);
        }

        public void Add(Staff staff)
        {
            _context.Staff.Add(staff);
            _context.SaveChanges();
        }

        public void Update(Staff staff)
        {
            if (_context.Entry(staff).State == EntityState.Detached)
            {
                _context.Staff.Attach(staff);
            }

            _context.Entry(staff).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void ToggleStatus(int staffId)
        {
            var staff = _context.Staff.Find(staffId);
            if (staff != null)
            {
                staff.IsActive = !staff.IsActive;
                _context.SaveChanges();
            }
        }
    }
}
