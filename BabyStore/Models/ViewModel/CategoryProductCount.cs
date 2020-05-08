using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BabyStore.Models
{
    public class CategoryWithCount
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }

        public string CatNameWithCount
        {
            get
            {
                return CategoryName + " (" + ProductCount.ToString() + ")";
            }
        }

    }
    public class CategoryProductCount
    {

        public IEnumerable<CategoryWithCount> CatsWithCount { get; set; }

        public IEnumerable<SelectListItem> CatFilterItems {

            get
            {
                return CatsWithCount.Select(cc =>new SelectListItem
                {
                    Text = cc.CatNameWithCount,
                    Value = cc.CategoryName
                });
                
            }
        }
    }
}
