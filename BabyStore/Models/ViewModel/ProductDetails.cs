using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BabyStore.Models
{
    public class ProductDetails
    {
        public Product Product { get; set; }
        public List<ProductImage> ProductImages { get; set; }
    }
}
