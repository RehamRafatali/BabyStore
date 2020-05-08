using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BabyStore.Models;
using BabyStore.Infrastrcture;
using Microsoft.AspNetCore.Identity;
using BabyStore.Infrastrcture;
namespace BabyStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly IEntityRepository<Order> repository;
        private readonly IEntityRepository<OrderLine> orderLineRepo;
        private readonly IEntityRepository<BasketLine> basketLineRepo;
        private readonly IEntityRepository<Product> productRepo;
        private readonly UserManager<AppUser> userManager;
        private readonly AppIdentityDbContext context;
        private readonly TotalAccount account;
        private readonly Basket basketService;

        public OrderController(IEntityRepository<Order> repository,
            IEntityRepository<OrderLine> orderLineRepo,
            IEntityRepository<BasketLine> basketLineRepo,
            IEntityRepository<Product> productRepo,
            UserManager<AppUser> userManager,
            AppIdentityDbContext context,
            TotalAccount account,
            Basket basketService)
        {
            this.repository = repository;
            this.orderLineRepo = orderLineRepo;
            this.basketLineRepo = basketLineRepo;
            this.productRepo = productRepo;
            this.userManager = userManager;
            this.context = context;
            this.account = account;
            this.basketService = basketService;
        }

        public IActionResult UserOders()
        {
          //IEnumerable<Order> orders= repository.FindBy(o => o.UserID == basketService.BasketId);
          var allOders= repository.AllIncluding(o => o.OrderLines);
           var UserOders= allOders.Where(o => o.UserID == basketService.BasketId);
            return View(UserOders);
        }
        public IActionResult OderDetails(int orderId)
        {
            ViewBag.Oder = repository.GetSingle(orderId);
         
            IEnumerable<OrderLine> orderLines= orderLineRepo.FindBy(ol => ol.OrderID == orderId);
            return View(orderLines);
        }

        public IActionResult Index()
        {
            AppUser user = userManager
                .FindByEmailAsync(basketService.BasketId).Result;
           Address address= context.Find<Address>(user.AddressId);

            var lines = basketLineRepo.FindBy(bl => bl.BasketID ==
                    basketService.BasketId)
                .Select(l => l.ProductID).ToList();

            var products = productRepo.FindBy(p =>
                lines.Contains(p.ID)).ToList<Product>();


            BasketViewModel viewModel = new BasketViewModel
            {
                BasketLines = basketLineRepo.FindBy(bl => bl.BasketID ==
                  basketService.BasketId).ToList(),
                Products = products,
                TotalCost = account.GetTotalCost(),
                Address=address
            };
            
                return View("CheckOutOrder", viewModel);
            
            

        }

        public async Task<IActionResult> Create()
        {
            string basketId = basketService.BasketId;
            AppUser user = await userManager.FindByEmailAsync(basketId);
            List<BasketLine> basketLines = basketLineRepo
                .FindBy(bl => bl.BasketID == basketId).ToList<BasketLine>();

            Order order = new Order();
            order.DeliveryName = user.UserName;
            //Address address= context.Find<Address>(user.AddressId);
            //order.DeliveryAddress = address;
            order.UserID = basketId;
            order.DateCreated = DateTime.Now;
            order.OrderLines = new List<OrderLine>();            
               foreach (BasketLine basketLine in basketLines)
               {
                Product product = account.GetProduct(basketLine.ProductID);
                OrderLine orderLine = new OrderLine
                {
                    
                    Product = product,
                    ProductID = product.ID,
                    ProductName = product.Name,
                    Quantity = basketLine.Quantity,
                    UnitPrice = product.Price
                };
                order.OrderLines.Add(orderLine);
            }
            order.TotalPrice = account.GetTotalCost();
            repository.Add(order);
            repository.Save();
            return View();
        }
    }
}