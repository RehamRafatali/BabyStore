using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BabyStore.Models;
using BabyStore.Infrastrcture;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace BabyStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly Basket basketService;
        private readonly IEntityRepository<BasketLine> repository;
        
        public AdminController(UserManager<AppUser> userManager
            ,RoleManager<IdentityRole> roleManager
            ,SignInManager<AppUser> signInManager
            ,Basket basketService
            ,IEntityRepository<BasketLine> repository)
        {
           
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.basketService = basketService;
            this.repository = repository;
        }

        

        public IActionResult Index()
        {
          
            return View(userManager.Users);
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel vModel,
           string returnUrl, string role = "User")
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { 
                UserName=vModel.UserName,
                Email = vModel.Email,
                Address = vModel.Address                
                };
                IdentityResult result = await
                    userManager.CreateAsync(user, vModel.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    MigrateBasket(vModel.Email);
                    return Redirect(returnUrl);

                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

               
            }
            return Redirect(returnUrl);

        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vModel,
            string returnUrl)
        {
            HttpContext.Session.Remove("BasketID");
            await signInManager.SignOutAsync();            
            
            
            AppUser user = await userManager.FindByEmailAsync(vModel.Email);
            if (user != null)
            {
               
                Microsoft.AspNetCore.Identity.SignInResult signInResult = await signInManager
                    .PasswordSignInAsync(user, vModel.Password, false, false);
                
                if (!signInResult.Succeeded)
                {
                    ModelState.AddModelError(nameof(LoginViewModel.Email),
                        "Invalid Email or Password");
                   
                }
                else
                {
                    MigrateBasket(vModel.Email);
                    return Redirect(returnUrl);
                }
            }
            return Redirect("/");

        }



        public async Task<IActionResult> Logout(string returnUrl)
        {
            HttpContext.Session.Remove("BasketID");
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);   
        }


        public void EmptyBasket()
        {
            var basketLines = repository.FindBy(bl => bl.BasketID ==
            basketService.BasketId);
            foreach (var basketLine in basketLines)
            {
                repository.Delete(basketLine);
            }
            repository.Save();

        }

        public void MigrateBasket(string userName)
        {
            var basket = repository.FindBy(bl => bl.BasketID ==
              basketService.BasketId).ToList();

            var userBasket = repository.FindBy(bl => bl.BasketID ==
             basketService.UserName).ToList();

            if (userBasket.Count>0)
            {
                string prevID = basketService.BasketId;
                basketService.BasketId = userName;
                foreach (var item in basket)
                {
                    item.BasketID = userName;
                    repository.Add(item);
                }
                repository.Save();
                basketService.BasketId = prevID;
                EmptyBasket();
              

            }
            else
            {
                if (basket.Count > 0)
                {
                    foreach (var item in basket)
                    {
                        item.BasketID = userName;
                    }
                    repository.Save();
                }

            }
            HttpContext.Session.SetString("BasketID", userName);


        }

    }
}