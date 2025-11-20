using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IActionResult Index(string searchString)
        {
            var categories = _repoCategory.GetAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                categories = categories
                    .Where(c => c.CategoryName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.SearchString = searchString;

            return View(categories);
        }

        // GET: load partial create
        public IActionResult Create()
        {
            return PartialView("_CreateCategoryPartial", new Category());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CategoryId,CategoryName,Decription")] Category category)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_CreateCategoryPartial", category);

                return View(category);
            }

            category.IsActive = true;
            _repoCategory.Add(category);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }

        // GET: load partial edit
        public IActionResult Edit(int id)
        {
            var category = _repoCategory.GetById(id);
            if (category == null) return NotFound();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_EditCategoryPartial", category);

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("CategoryId,CategoryName,Decription")] Category category)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_EditCategoryPartial", category);
                return View(category);
            }
            category.IsActive = true;
            _repoCategory.Update(category);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }


        // GET: load partial delete
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = _repoCategory.GetById(id.Value);
            if (category == null) return NotFound();

            return PartialView("_DeleteCategoryPartial", category);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repoCategory.Delete(id);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }
    }
}
