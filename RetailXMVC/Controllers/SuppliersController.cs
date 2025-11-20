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

        public IActionResult Index(string searchString)
        {
            var suppliers = _repoSupplier.GetAllSuppliers();

            if (!string.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(s =>
                    s.SupplierName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    s.Email.Contains(searchString) ||
                    s.Phone.Contains(searchString)
                ).ToList();
            }

            ViewBag.SearchString = searchString;
            return View(suppliers);
        }

        // GET Create Partial
        public IActionResult Create()
        {
            return PartialView("_CreateSupplierPartial", new Supplier());
        }

        [HttpPost]
        public IActionResult Create(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateSupplierPartial", supplier);

            supplier.IsActive = true;
            _repoSupplier.AddSupplier(supplier);

            return Json(new { success = true });
        }

        // GET Edit Partial
        public IActionResult Edit(int id)
        {
            var supplier = _repoSupplier.GetSupplierById(id);
            return PartialView("_EditSupplierPartial", supplier);
        }

        [HttpPost]
        public IActionResult Edit(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return PartialView("_EditSupplierPartial", supplier);

            supplier.IsActive = true;
            _repoSupplier.UpdateSupplier(supplier);

            return Json(new { success = true });
        }

        // GET: load partial delete
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var supplier = _repoSupplier.GetSupplierById(id.Value);
            if (supplier == null) return NotFound();

            return PartialView("_DeleteSupplierPartial", supplier);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repoSupplier.DeleteSupplier(id);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }


    }
}
