using BusinessObject.Models;
using DataAccessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly StaffDAO _staffDAO;

        public StaffRepository(StaffDAO staffDAO)
        {
            _staffDAO = staffDAO;
        }

        public List<Staff> GetStaffListForOwner()
        {
            return _staffDAO.GetAll().OrderByDescending(s => s.IsActive).ToList();
        }

        public Staff GetStaffDetail(int staffId)
        {
            return _staffDAO.GetById(staffId);
        }

        public void CreateStaff(Staff staff)
        {
            _staffDAO.Add(staff);
        }

        public void UpdateStaff(Staff staff)
        {
            _staffDAO.Update(staff);
        }

        public void ToggleStaffStatus(int staffId)
        {
            _staffDAO.ToggleStatus(staffId);
        }
    }
}
