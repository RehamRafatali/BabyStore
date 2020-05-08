using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BabyStore.Controllers
{
    public class StripeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}