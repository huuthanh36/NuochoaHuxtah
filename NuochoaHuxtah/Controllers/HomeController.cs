using Microsoft.AspNetCore.Mvc;
using NuochoaHuxtah.Models;
using NuochoaHuxtah.Repository;
using System.Diagnostics;

namespace NuochoaHuxtah.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext; 
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = _dataContext.Products.ToList();
            
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

		public IActionResult Contact()
		{
			
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if(statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            
        }
    }
}
