using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BabyStore.Models
{
    public class BasketViewModel
    {
        public List<Product> Products { get; set; }
        public List<BasketLine> BasketLines { get; set; }
        [Display(Name = "Basket Total:")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal TotalCost { get; set; }
        public Address Address { get; set; }
    }
}
