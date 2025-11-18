using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ISupplierRepository _repoSupplier;

        public SuppliersController(ISupplierRepository repoSupplier)
        {
            _repoSupplier = repoSupplier;
        }

        // GET: Suppliers
        public IActionResult Index()
        {
            var suppliers = _repoSupplier.GetAllSuppliers();
            return View(suppliers);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("SupplierId,SupplierName,Phone,Email,Address")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                supplier.IsActive = true;
                _repoSupplier.AddSupplier(supplier);
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var supplier = _repoSupplier.GetSupplierById(id.Value);
            if (supplier == null) return NotFound();

            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("SupplierId,SupplierName,Phone,Email,Address")] Supplier supplier)
        {
            if (id != supplier.SupplierId) return NotFound();

            if (ModelState.IsValid)
            {
                supplier.IsActive = true;
                _repoSupplier.UpdateSupplier(supplier);
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var supplier = _repoSupplier.GetSupplierById(id.Value);
            if (supplier == null) return NotFound();

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repoSupplier.DeleteSupplier(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
