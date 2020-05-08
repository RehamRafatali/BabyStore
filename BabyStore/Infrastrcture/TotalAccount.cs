using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BabyStore.Models;
namespace BabyStore.Infrastrcture
{
    public class TotalAccount
    {
        private readonly IEntityRepository<BasketLine> repository;
        private readonly IEntityRepository<Product> prodRepo;
        private readonly Basket basketService;

        public  TotalAccount(IEntityRepository<BasketLine> repository,
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

        public Product GetProduct(int id)
        {
           return prodRepo.GetSingle(id);

        }


    }
}
