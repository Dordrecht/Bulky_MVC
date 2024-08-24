using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        // Variable to hold our db implementation 
        private readonly IUnitOfWork _unitOfWork;

        // Receive the db through dependency injection, and then assign it to our _db field. 
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            // Retrieve the list of Categories - all in one line
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            // Note the objCategoryList var being passed to the view below.
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    // We can create a new custom error using AddModelError. It allows us to specify a key and a message.
            //    ModelState.AddModelError("name", "The Display Order cannot exactly match the name.");
            //}
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                // TempData holds data only until the next render.
                TempData["success"] = "Category created successfully";
                // RedirectToAction sends processing to the Index action where we can view the new category in the list.
                // We only specify the action if we are going to another action in the same controller, but if to
                // a different controller, that can be specified in a second parameter.
                return RedirectToAction("Index");
            }
            return View();
        }

        // We need the Id of the category we want to edit.
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // We want to have EFC return only one category. There are multiple ways of doing that:
            // Method #1: Find. Find works off the primary key of the model.
            // Category? categoryFromDb = _db.Categories.Find(id);
            // Method #2: FirstOrDefault uses a link operation. 
            // Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
            // Method #2b: FirstOrDefault doesn't have to look for the primary key. 
            // Category? categoryFromDb2 = _db.Categories.FirstOrDefault(u=>u.Name == "SciFi");
            // Method #2c: FirstOrDefault can use a Contains.
            // Category? categoryFromDb3 = _db.Categories.FirstOrDefault(u=>u.Name.Contains("Sci"));
            // Method #3: Where.
            // Category? categoryFromDb4 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();
            // New way we added later to do the above because we have the CategoryRepository later...
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        // We need the Id of the category we want to edit.
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (id == null || id == 0)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
