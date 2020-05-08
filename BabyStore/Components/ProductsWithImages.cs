using BabyStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BabyStore.Components
{
    public class ProductsWithImages : ViewComponent
    {
        private readonly StoreContext context;

        public ProductsWithImages(StoreContext context)
        {
            this.context = context;
        }
        public IViewComponentResult Invoke(int id)
        {
            var product = context.Products.Where(p => p.ID == id)
                .Include(p => p.ProductImageMappings)
                .ThenInclude(pim => pim.ProductImage).First();
            // var product= context.Products.Find(id);
            return View(product);
        }
    }
}
