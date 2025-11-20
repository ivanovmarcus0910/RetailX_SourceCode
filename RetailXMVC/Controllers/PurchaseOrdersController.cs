using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        private readonly IPurchaseOrderRepository _repoOrder;
        private readonly IPurchaseOrderDetailRepository _repoDetail;
        private readonly IProductRepository _repoProduct;
        private readonly ISupplierRepository _repoSupplier;

        public PurchaseOrdersController(
            IPurchaseOrderRepository orderRepo,
            IPurchaseOrderDetailRepository detailRepo, IProductRepository repoProduct,
            ISupplierRepository repoSupplier)
        {
            _repoOrder = orderRepo;
            _repoDetail = detailRepo;
            _repoProduct = repoProduct;
            _repoSupplier = repoSupplier;
        }
        public IActionResult Index(string searchSupplier, int? statusFilter, DateTime? fromDate, DateTime? toDate)
        {
            var orders = _repoOrder.GetAll().AsQueryable();

            // Search theo supplier
            if (!string.IsNullOrEmpty(searchSupplier))
                orders = orders.Where(o => o.Supplier != null &&
                                           o.Supplier.SupplierName.Contains(searchSupplier));

            // Filter status
            if (statusFilter.HasValue)
                orders = orders.Where(o => o.Status == statusFilter.Value);

            // Filter From Date
            if (fromDate.HasValue)
                orders = orders.Where(o => o.CreateDate >= fromDate.Value);

            // Filter To Date
            if (toDate.HasValue)
                orders = orders.Where(o => o.CreateDate <= toDate.Value);

            // SORT: Ngày mới nhất lên đầu
            orders = orders.OrderByDescending(o => o.CreateDate);

            // Lưu vào ViewBag để giữ giá trị sau khi submit
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.SearchSupplier = searchSupplier;

            // Dropdown Status
            ViewBag.StatusList = new SelectList(new[]
            {
        new { Value = "", Text = "All Status" },
        new { Value = "2", Text = "Pending" },
        new { Value = "3", Text = "Confirmed" }
    }, "Value", "Text", statusFilter?.ToString());

            return View(orders.ToList());
        }



        // GET: PurchaseOrders/Details/5
        public IActionResult Details(int id)
        {
            var order = _repoOrder.GetById(id);
            if (order == null) return NotFound();

            ViewBag.Details = _repoDetail.GetByOrder(id);

            return View(order);
        }

        // GET: PurchaseOrders/Create
        public IActionResult Create()
        {
            ViewBag.Suppliers = _repoSupplier.GetAllSuppliers();
            return View();
        }

        // POST: PurchaseOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("SupplierId,CreateDate")] PurchaseOrder order)
        {
            if (ModelState.IsValid)
            {
                order.IsActive = true;
                order.Status = 2;
                _repoOrder.Add(order);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Suppliers = _repoSupplier.GetAllSuppliers();
            return View(order);
        }

        // GET: PurchaseOrders/Edit/5
        public IActionResult Edit(int id)
        {
            var order = _repoOrder.GetById(id);
            if (order == null) return NotFound();

            ViewBag.Suppliers = _repoSupplier.GetAllSuppliers();
            return View(order);
        }

        // POST: PurchaseOrders/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PurchaseOrder order)
        {
            if (id != order.PurchaseOrderId) return NotFound();

            if (ModelState.IsValid)
            {
                _repoOrder.Update(order);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Suppliers = _repoSupplier.GetAllSuppliers();
            return View(order);
        }

        // GET: PurchaseOrders/Delete/5
        public IActionResult Delete(int id)
        {
            var order = _repoOrder.GetById(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: PurchaseOrders/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repoOrder.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: PurchaseOrders/Approve/5
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var order = _repoOrder.GetById(id);
            if (order == null) return NotFound();

            // Lấy danh sách chi tiết
            var details = _repoDetail.GetByOrder(id);

            // Cộng Quantity vào Product
            foreach (var d in details)
            {
                var product = _repoProduct.GetById(d.ProductId);
                if (product != null)
                {
                    product.Quantity += d.Quantity ?? 0;
                    _repoProduct.Update(product);
                }
            }

            order.Status =3;
            _repoOrder.Update(order);

            return RedirectToAction(nameof(Index));
        }
    }
}
