using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _repoProduct;
        private readonly ICategoryRepository _repoCategory;
        private readonly ISupplierRepository _repoSupplier;


        public ProductsController(IProductRepository repo, ICategoryRepository repoCategory, ISupplierRepository repoSupplier)
        {
            _repoProduct = repo;
            _repoCategory = repoCategory;
            _repoSupplier = repoSupplier;
        }

        // GET: Products
        public IActionResult Index(string searchString, int? categoryId)
        {
            var products = _repoProduct.GetAll(); // lấy tất cả

            // Filter theo category
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }

            // Search theo tên sản phẩm
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products
                    .Where(p => p.ProductName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Load dropdown Category
            ViewBag.Categories = new SelectList(_repoCategory.GetAll(), "CategoryId", "CategoryName");

            // Giữ lại giá trị filter/search
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SearchString = searchString;

            return View(products);
        }



        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_repoCategory.GetAll(), "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_repoSupplier.GetAllSuppliers(), "SupplierId", "SupplierName");
            return PartialView("_CreateProductPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                // reload dropdown
                ViewBag.CategoryId = new SelectList(_repoCategory.GetAll(), "CategoryId", "CategoryName", product.CategoryId);
                ViewBag.SupplierId = new SelectList(_repoSupplier.GetAllSuppliers(), "SupplierId", "SupplierName", product.SupplierId);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_CreateProductPartial", product); // trả partial nếu validation lỗi

                return View(product);
            }

            _repoProduct.Insert(product);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = _repoProduct.GetById(id.Value);
            if (product == null) return NotFound();

            ViewBag.CategoryId = new SelectList(_repoCategory.GetAll(), "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.SupplierId = new SelectList(_repoSupplier.GetAllSuppliers(), "SupplierId", "SupplierName", product.SupplierId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_EditProductPartial", product);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("ProductId,ProductName,CategoryId,SupplierId,Price,Quantity")] Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_repoCategory.GetAll(), "CategoryId", "CategoryName", product.CategoryId);
                ViewBag.SupplierId = new SelectList(_repoSupplier.GetAllSuppliers(), "SupplierId", "SupplierName", product.SupplierId);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_EditProductPartial", product);

                return View(product);
            }

            product.IsActive = true;
            _repoProduct.Update(product);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }

        // GET: load modal
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = _repoProduct.GetById(id.Value);
            if (product == null) return NotFound();

            // trả về partial view nếu AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_DeleteProductPartial", product);

            return View(product); // fallback nếu ko AJAX
        }

        // POST: thực sự xóa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repoProduct.Delete(id);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }

    }
}
