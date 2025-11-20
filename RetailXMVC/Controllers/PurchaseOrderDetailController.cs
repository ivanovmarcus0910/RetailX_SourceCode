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

        // GET: load partial create form
        public IActionResult Create(int orderId, int supplierId)
        {
            ViewBag.OrderId = orderId;
            ViewBag.Products = _repoProduct.GetProductsBySupplier(supplierId);
            return PartialView("_AddProductPartial");
        }

        // POST: add product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int orderId, int productId, int quantity, decimal price)
        {
            var product = _repoProduct.GetById(productId);
            if (product == null) return BadRequest();

            // Kiểm tra sản phẩm đã tồn tại trong PO chưa
            var existingDetail = _repoDetail.GetByOrder(orderId)
                                            .FirstOrDefault(d => d.ProductId == productId);

            if (existingDetail != null)
            {
                existingDetail.Quantity += quantity; // cộng dồn số lượng
                existingDetail.Price = price; // hoặc giữ giá cũ nếu muốn
                _repoDetail.Update(existingDetail); // cần tạo method Update trong repository
            }
            else
            {
                var detail = new PurchaseOrderDetail
                {
                    PurchaseOrderId = orderId,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = price
                };
                _repoDetail.Add(detail);
                existingDetail = detail;
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    detailId = existingDetail.PurchaseOrderDetailId,
                    productName = product.ProductName,
                    quantity = existingDetail.Quantity,
                    price = existingDetail.Price,
                    total = existingDetail.Quantity * existingDetail.Price
                }
            });
        }

        public IActionResult Edit(int detailId)
        {
            var detail = _repoDetail.GetById(detailId);
            if (detail == null) return NotFound();
            return PartialView("_EditProductPartial", detail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int detailId, int quantity, decimal price)
        {
            var detail = _repoDetail.GetById(detailId);
            if (detail == null) return NotFound();

            detail.Quantity = quantity;
            detail.Price = price;
            _repoDetail.Update(detail);

            return Json(new { success = true });
        }

        // --- Delete Product Partial ---
        public IActionResult Delete(int detailId)
        {
            var detail = _repoDetail.GetById(detailId);
            if (detail == null) return NotFound();
            return PartialView("_DeleteConfirmPartial", detail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int detailId, int orderId)
        {
            _repoDetail.Delete(detailId);
            return Json(new { success = true });
        }
    }
}
