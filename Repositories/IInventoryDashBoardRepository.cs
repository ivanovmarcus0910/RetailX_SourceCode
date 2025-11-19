using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repositories
{
    public interface IInventoryDashBoardRepository
    {
        int GetTotalProducts();
        int GetTotalCategories();
        int GetTotalSuppliers();
        int GetTotalCustomers();

        decimal GetInventoryValue();
        List<Product> GetLowStock();
        List<object> GetMonthlyImport();

        List<dynamic> GetImportReportDetail(
        int? year = null,
        int? month = null,
        int? supplierId = null,
        int? categoryId = null
    );

    }
}
