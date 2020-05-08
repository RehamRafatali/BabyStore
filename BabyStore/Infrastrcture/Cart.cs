using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BabyStore.Models;
namespace BabyStore.Infrastrcture
{
    public class Cart
    {
        private List<CartLine> cartLines = new List<CartLine>();

        public virtual void AddItem(int productId,int quantity)
        {
            CartLine line = cartLines.Where(cl => cl.ProductId == productId)
                 .FirstOrDefault();
            if (line == null)
            {
                cartLines.Add(new CartLine
                {
                    ProductId = productId,
                    Quantity = quantity

                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }
    }
    public class CartLine
    {
        public int CartLineId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
