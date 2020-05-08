using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BabyStore.Models;
namespace BabyStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IEntityRepository<Category> repository;

        public CategoryController(IEntityRepository<Category> repository)
        {

            this.repository = repository;
        }
        public IActionResult Index()
        {
            ViewBag.CategoryId = TempData["CategoryId"];
            //  PaginatedList<Category> cts= repository.Paginate(1, 2, s => s.Name, null, null);
            return View(repository.GetAll());
        }
        //public IActionResult Index2()
        //{
        //   PaginatedList<Category> cts= repository.Paginate(1, 2, s => s.Name, null);
        //    return View(cts);
        //}
        //[HttpPost]
        //public IActionResult Create(Category category)
        //{
        //    repository.Add(category);
        //    repository.Save();
        //    return RedirectToAction(nameof(Index));
        //}

        public IActionResult Edit(int id)
        {
            TempData["CategoryId"] = id;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (category.ID == 0)
            {
                repository.Add(category);
            }
            else
            {
                repository.Edit(category);
            }
            repository.Save();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public  IActionResult Delete(int id)
        {
          Category c= repository.GetSingle(id);
            repository.Delete(c);
            repository.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}