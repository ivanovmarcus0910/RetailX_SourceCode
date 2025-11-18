using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _repoCategory;

        public CategoriesController(ICategoryRepository repoCategory)
        {
            _repoCategory = repoCategory;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var categories = _repoCategory.GetAll();
            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CategoryId,CategoryName,Decription")] Category category)
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
                return View(category);
            }

            category.IsActive = true; // nếu dùng soft delete
            _repoCategory.Add(category);
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = _repoCategory.GetById(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("CategoryId,CategoryName,Decription")] Category category)
        {
            if (id != category.CategoryId) return NotFound();

            if (ModelState.IsValid)
            {
                category.IsActive = true;
                _repoCategory.Update(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = _repoCategory.GetById(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // soft delete
            _repoCategory.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
