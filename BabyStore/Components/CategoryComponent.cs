using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BabyStore.Models;

namespace BabyStore.Components
{
    public class CategoryComponent:ViewComponent
    {
        private readonly IEntityRepository<Category> repository;
        private readonly IEntityRepository<Product> productRepo;

        public CategoryComponent(IEntityRepository<Category> repository,
            IEntityRepository<Product> productRepo)
        {
            this.repository = repository;
            this.productRepo = productRepo;
        }
        public IViewComponentResult Invoke()
        {
            var products = productRepo.AllIncluding(p => p.Category);
            CategoryProductCount vModel = new CategoryProductCount
            {
                CatsWithCount=from m in products
                              where m.CategoryID !=null
                              group m by m.Category.Name 
                              into catGrop
                              select new CategoryWithCount
                              {
                                   CategoryName=catGrop.Key,
                                    ProductCount=catGrop.Count()
                              }
            };
            return View(vModel);
        }
    }
}
