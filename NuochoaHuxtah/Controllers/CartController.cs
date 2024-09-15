using Microsoft.AspNetCore.Mvc;

namespace NuochoaHuxtah.Controllers
{
    public class CartController : Controller 
    {
        public IActionResult Index()
        {
            return View();
        }

		public IActionResult Checkout()
		{
			return View("~/Views/Checkout/Index.cshtml");
		}
	}
}
