using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class StaffDAO
    {
        private readonly Tenant0Context _context;

        public StaffDAO(Tenant0Context context)
        {
            _context = context;
        }

        public async Task<List<Staff>> GetAllActiveStaffs()
        {
            return await _context.Staff
                                 .Where(s => s.BaseSalary.HasValue)
                                 .ToListAsync();
        }

        public async Task<Staff> GetStaffById(int staffId)
        {
            // Lấy thông tin nhân viên theo ID
            return await _context.Staff
                                 .FirstOrDefaultAsync(s => s.StaffId == staffId);
        }
    }
}