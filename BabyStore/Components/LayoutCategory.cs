using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BabyStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BabyStore.Components
{
    public class LayoutCategory: ViewComponent
    {
        private readonly IEntityRepository<Category> repository;

        public LayoutCategory(IEntityRepository<Category> repository)
        {
              this.repository = repository;
        }

        public IViewComponentResult Invoke(bool nav=false)
        {
            if (nav)
            {
                return View("NavList",repository.GetAll());
            }
            else
            {
                IEnumerable<Category> categories = repository
                    .AllIncluding(c => c.Products);

                //SelectList x = new SelectList(categories,"ID","Name");

                return View(categories);
            }
        }
    }
}
