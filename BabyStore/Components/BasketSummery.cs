using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BabyStore.Models;
using BabyStore.Infrastrcture;
namespace BabyStore.Components
{
    public class BasketSummery: ViewComponent
    {
        private readonly IEntityRepository<BasketLine> repository;
        private readonly IEntityRepository<Product> prodRepo;
        private readonly Basket basketService;

        public BasketSummery(IEntityRepository<BasketLine> repository,
            IEntityRepository<Product> prodRepo,
            Basket basketService)
        {
            this.repository = repository;
            this.prodRepo = prodRepo;
            this.basketService = basketService;
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
        public IViewComponentResult Invoke()
        {
            return View(new BasketSummaryViewModel
            {
                NumberOfItems = GetNumberOfItems(),
                TotalCost=GetTotalCost()
            }) ;

        }
    }
}
