using BusinessObject.Models;

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
