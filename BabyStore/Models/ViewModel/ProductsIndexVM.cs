using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BabyStore.Models
{
    public class ProductsIndexVM
    {
        public PaginatedList<Product> Products { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string SortBy { get; set; }
        //   public string Name { get; set; }
    }
}
