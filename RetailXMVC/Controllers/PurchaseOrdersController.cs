using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            var orders = _repoOrder.GetAll();
            return View(orders);
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
