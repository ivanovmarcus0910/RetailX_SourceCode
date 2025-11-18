using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class PurchaseOrderDetailController : Controller
    {
        private readonly IPurchaseOrderDetailRepository _repoDetail;
        private readonly IProductRepository _repoProduct;

        public PurchaseOrderDetailController(IPurchaseOrderDetailRepository repoDetail, IProductRepository repoProduct)
        {
            _repoDetail = repoDetail;
            _repoProduct = repoProduct;
        }

        // GET: Add detail
        public IActionResult Create(int orderId, int supplierId)
        {
            ViewBag.OrderId = orderId;
            ViewBag.Products = _repoProduct.GetProductsBySupplier(supplierId);

            return View();
        }

        // POST: Add detail
        [HttpPost]
        public IActionResult Create(int orderId, int productId, int quantity, decimal price)
        {
            PurchaseOrderDetail detail = new()
            {
                PurchaseOrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
                Price = price
            };

            _repoDetail.Add(detail);
            return RedirectToAction("Details", "PurchaseOrders", new { id = orderId });
        }

        public IActionResult Delete(int id, int orderId)
        {
            _repoDetail.Delete(id);
            return RedirectToAction("Details", "PurchaseOrders", new { id = orderId });
        }
    }
}
