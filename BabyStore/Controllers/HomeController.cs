using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BabyStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BabyStore.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly IEntityRepository<Product> repository;
        private readonly IEntityRepository<Category> catRepo;
        private readonly IEntityRepository<ProductImage> imgRepo;
        private readonly IEntityRepository<ProductImageMapping> mapRepo;

        public HomeController(IEntityRepository<Product> repository,
            IEntityRepository<Category> catRepo,
            IEntityRepository<ProductImage> imgRepo
            , IEntityRepository<ProductImageMapping> mapRepo)
        {
            this.repository = repository;
            this.catRepo = catRepo;
            this.imgRepo = imgRepo;
            this.mapRepo = mapRepo;
        }
        public IActionResult Index(int categoryId, string sortBy,
            int page = 1,
            bool All=false)
        {

            ProductsIndexVM model = new ProductsIndexVM
            {
                CategoryId = categoryId,
                Categories = catRepo.GetAll().ToList<Category>()                
            };
            model.SortBy = sortBy;
            if (categoryId == default(int))
            {
                model.Products = repository
                   .Paginate(page,repository.GetAll().Count(), p => p.ID,null,m=>m.ProductImageMappings);
            }
            else
            {
                model.Products = repository
                  .Paginate(page, Constants.PageSize, p => p.ID,
                  p => p.CategoryID == categoryId,m => m.ProductImageMappings);
              
            }
            ViewBag.images = imgRepo.GetAll();
            if (All == true)
            {
                ViewBag.All = true;
                model.Products = repository
                  .Paginate(page, 4, p => p.Price,
                  p => p.CategoryID == categoryId,
                  m => m.ProductImageMappings);

                switch (sortBy)
                {
                    case "Low":
                        break;
                    case "High":
                        model.Products.Reverse();
                        break;
                    default:

                        break;
                }
                return View(model);
            }
            else
            {
                return View(model);
            }
            

        }

        public IActionResult Details(int id, string returnUrl)
        {
            Product product = repository.AllIncluding(
                p => p.ProductImageMappings,x=>x.Category).First(p => p.ID == id);
            int[] mappingIds = product.ProductImageMappings
                .Select(pim => pim.ProductImageID).ToArray();

           List<ProductImage> images= imgRepo
                .FindBy(pi => mappingIds.Contains(pi.ID)).ToList();
            return View(new ProductDetails { 
             Product=product,
             ProductImages=images
            });
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
