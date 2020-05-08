using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BabyStore.Models;
namespace BabyStore.Infrastrcture
{
    public class Basket
    {
       
        private const string BasketSessionKey = "BasketID";
        //private readonly IEntityRepository<BasketLine> repository;
      //  private readonly IEntityRepository<Product> proRepo;
        public string BasketId;
        public ISession Session { get; set; }
        public string UserName { get; set; }
        //  public IEntityRepository<BasketLine> Repository { get; }

        public Basket() { }
       
       
        private string GetBasketId()
        {
            if (Session.GetString(BasketSessionKey)==null)
            {
                if (UserName != null)
                {
                    Session.SetString(BasketSessionKey, UserName);
                }
                else
                {
                    string tempBasketId = Guid.NewGuid().ToString();
                    Session.SetString(BasketSessionKey, tempBasketId);
                }
            }
          return  Session.GetString(BasketSessionKey);
        }
        public static Basket GetBasket(IServiceProvider services)
        {
          ISession session= services.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;
            string userName = services.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.User.Identity.Name;
            
            Basket basket = new Basket();
            basket.Session = session;
            basket.UserName = userName;
            basket.BasketId =session.GetString(BasketSessionKey)??basket.GetBasketId();
           
            return basket;
        }

        

    }
}
