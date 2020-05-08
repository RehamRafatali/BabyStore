using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BabyStore.Models;
using Microsoft.AspNetCore.Http;
using BabyStore.Infrastrcture;
using BabyStore.Extensions;
namespace BabyStore.Controllers
{
    public class CartController : Controller
    {

        private readonly IEntityRepository<BasketLine> repository;
        
        public CartController(IEntityRepository<BasketLine> repository)
        {
            this.repository = repository;
        }
        public IActionResult Index()
        {
            string id =HttpContext.Session.GetString("Cart")?? "no data";
            return View((object)id);
        }
        public IActionResult Add(int p=11,int q = 3)
        { 
            string cart = GetCart();
            BasketLine basketLine = new BasketLine
            {
                BasketID = cart,
                ProductID = p,
                Quantity = q
            };
            repository.Add(basketLine);
            repository.Save();
            SaveCart(cart);           
            return RedirectToAction(nameof(Index));
        }

        private string GetCart()
        {
            if (HttpContext.Session.GetString("Cart") == null)
            {
                return Guid.NewGuid().ToString();
            }
            else
            {
                return HttpContext.Session.GetString("Cart");
            }
            
        }
        private void SaveCart(string cart)
        {
            HttpContext.Session.SetString("Cart", cart);
        }

        //private string GetCartId()
        //{

        //    if (HttpContext.Session.GetString("CartId")==null)
        //    {
        //        if (!string.IsNullOrWhiteSpace(HttpContext.User.Identity.Name))
        //        {
        //            HttpContext.Session.SetString("CartId",
        //                HttpContext.User.Identity.Name);
        //        }
        //        else
        //        {
        //            Guid tempCartId = Guid.NewGuid();
        //            HttpContext.Session.SetString("CartId",
        //                tempCartId.ToString());
        //        }
        //    }
        //        return HttpContext.Session.GetString("CartId");
        //}

        //private void SaveCartId(string cartId)
        //{
        //    HttpContext.Session.SetString("CartId", cartId);
        //}
    }
}