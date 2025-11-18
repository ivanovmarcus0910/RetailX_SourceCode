using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IStaffRepository
    {
        List<Staff> GetStaffListForOwner();
        Staff GetStaffDetail(int staffId);
        void CreateStaff(Staff staff);
        void UpdateStaff(Staff staff);
        void ToggleStaffStatus(int staffId);
    }
}
