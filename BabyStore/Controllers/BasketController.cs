using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BabyStore.Infrastrcture;
using BabyStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BabyStore.Controllers
{
    public class BasketController : Controller
    {
        private readonly IEntityRepository<BasketLine> repository;
        private readonly IEntityRepository<Product> prodRepo;        
        private readonly Basket basketService;
        private readonly UserManager<AppUser> userManager;

        public BasketController(IEntityRepository<BasketLine> repository,
            IEntityRepository<Product> prodRepo,            
            Basket basketService,
            UserManager<AppUser> userManager)
        {
            this.repository = repository;
            this.prodRepo = prodRepo;
            
            this.basketService = basketService;
            this.userManager = userManager;
        }
        public IActionResult Index(string returnUrl)
        {            
            var lines = repository.FindBy(bl => bl.BasketID ==
                    basketService.BasketId)
                .Select(l=>l.ProductID).ToList();

            var products = prodRepo.FindBy(p =>            
                lines.Contains(p.ID)).ToList<Product>();


            BasketViewModel viewModel = new BasketViewModel {
                BasketLines = repository.FindBy(bl => bl.BasketID ==
                  basketService.BasketId).ToList(),
                Products=products,
                 TotalCost=GetTotalCost()
            };
            ViewBag.returnUrl = returnUrl;
           
            return View(viewModel);
            
        }

        


        [HttpPost]
        public IActionResult Add(int productID, int quantity=1,
            string returnUrl="")
        {
           
            BasketLine basketLine = repository.FindBy(bl =>
                bl.Product.ID == productID && bl.BasketID ==
                basketService.BasketId)
                  .FirstOrDefault();
            if (basketLine == null)
            {
                repository.Add(new BasketLine
                {
                    BasketID = basketService.BasketId,
                    ProductID = productID,
                    Quantity = quantity,
                    DateCreated = DateTime.Now
                });
            }
            else
            {
                basketLine.Quantity += quantity;
            }
            repository.Save();
            //basketService.AddToBasket(productID, quantity);
            return RedirectToAction(nameof(Index), new { returnUrl = returnUrl });
        
        }

        public IActionResult Delete(int productID)
        {
            BasketLine basketLine = repository.FindBy(bl =>
                bl.Product.ID == productID && bl.BasketID ==
                basketService.BasketId)
                  .FirstOrDefault();
            if (basketLine != null)
            {
                repository.Delete(basketLine);
            }
            repository.Save();
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult UpdateBasket(List<BasketLine> lines, string returnUrl="")
        {
            foreach (BasketLine line in lines)
            {
                BasketLine basketLine = repository.FindBy(bl=>bl.BasketID==
                line.BasketID && bl.ProductID==line.ProductID).FirstOrDefault();
                if (basketLine != null)
                {
                    if (line.Quantity == 0)
                    {
                        repository.Delete(basketLine);
                        //repository.Detache(basketLine);
                    }
                    else
                    {
                        basketLine.Quantity = line.Quantity;
                    }
                }               
            }
            repository.Save();
            return RedirectToAction(nameof(Index), new { returnUrl = returnUrl });
        }

        

       
        public decimal GetTotalCost()
        {
            decimal basketTotal = decimal.Zero;

            List<BasketLine> lines = repository.FindBy(bl =>
              bl.BasketID == basketService.BasketId).ToList<BasketLine>();

            if (lines.Count > 0)
            {
                foreach (BasketLine line in lines)
                {
                    var price = prodRepo.GetSingle(line.ProductID).Price;
                    basketTotal += price * line.Quantity;                        
                }
               
            }
            return basketTotal;
        }
        public int GetNumberOfItems()
        {
            int numberOfItems = 0;
            List<BasketLine> lines = repository.FindBy(bl =>
              bl.BasketID == basketService.BasketId).ToList<BasketLine>();
            if (lines.Count > 0)
            {
                numberOfItems = lines.Sum(b =>
                b.Quantity);
            }
            return numberOfItems;
        }


      
       
    }
    
    

}