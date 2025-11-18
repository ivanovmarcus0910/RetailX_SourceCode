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
        public IActionResult Index()
        {
            var products = _repoProduct.GetAll();
            return View(products);
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var product = _repoProduct.GetById(id.Value);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_repoCategory.GetAll(), "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_repoSupplier.GetAllSuppliers(), "SupplierId", "Address");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ProductId,ProductName,CategoryId,SupplierId,Price")] Product product)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"MODELSTATE ERROR → FIELD: {modelState.Key} | MESSAGE: {error.ErrorMessage}");
                    }
                }

                // Reload dropdowns
                ViewData["CategoryId"] = new SelectList(_repoCategory.GetAll(), "CategoryId", "CategoryName");
                ViewData["SupplierId"] = new SelectList(_repoSupplier.GetAllSuppliers(), "SupplierId", "SupplierName");

                return View(product);
            }

            _repoProduct.Insert(product);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = _repoProduct.GetById(id.Value);
            if (product == null) return NotFound();

            // Load categories
            ViewBag.CategoryId = new SelectList(
                _repoCategory.GetAll(),
                "CategoryId",
                "CategoryName",
                product.CategoryId // chọn đúng item hiện tại
            );

            // Load suppliers
            ViewBag.SupplierId = new SelectList(
                _repoSupplier.GetAllSuppliers(),
                "SupplierId",
                "SupplierName",
                product.SupplierId
            );

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ProductId,ProductName,CategoryId,SupplierId,Price")] Product product)
        {
            if (id != product.ProductId) return NotFound();

            if (ModelState.IsValid)
            {
                _repoProduct.Update(product);
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdowns khi lỗi validation
            ViewBag.CategoryId = new SelectList(
                _repoCategory.GetAll(),
                "CategoryId",
                "CategoryName",
                product.CategoryId
            );

            ViewBag.SupplierId = new SelectList(
                _repoSupplier.GetAllSuppliers(),
                "SupplierId",
                "SupplierName",
                product.SupplierId
            );

            return View(product);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = _repoProduct.GetById(id.Value);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repoProduct.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
