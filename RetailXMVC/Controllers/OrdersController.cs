using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;

        public OrdersController(IOrderRepository orderRepo, IProductRepository productRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
        }

        // =============================
        // GET /Orders
        // =============================
        public IActionResult Index()
        {
            var orders = _orderRepo.GetAll();
            return View(orders);
        }

        // =============================
        // GET /Orders/Details/5
        // =============================
        public IActionResult Details(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // =============================
        // GET /Orders/Create
        // =============================
        public IActionResult Create()
        {
            ViewBag.Products = _productRepo.GetAll();
            return View();
        }

        // =============================
        // POST /Orders/Create
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order, int[] productIds, int[] quantities)
        {
            if (productIds.Length != quantities.Length)
            {
                ModelState.AddModelError("", "Sản phẩm và số lượng không khớp!");
                ViewBag.Products = _productRepo.GetAll();
                return View(order);
            }

            // Basic info
            order.CreateDate = DateTime.Now;
            order.Status = 1;

            // Add order details
            for (int i = 0; i < productIds.Length; i++)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = productIds[i],
                    Quantity = quantities[i]
                });
            }

            _orderRepo.Insert(order);
            return RedirectToAction(nameof(Index));
        }

        // =============================
        // GET /Orders/Delete/5
        // =============================
        public IActionResult Delete(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // =============================
        // POST /Orders/DeleteConfirmed
        // =============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _orderRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
