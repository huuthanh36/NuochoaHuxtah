using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuochoaHuxtah.Repository;

namespace NuochoaHuxtah.Areas.Admin.Controllers
{
	[Area("admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly DataContext _dataContext;
		public OrderController(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IActionResult> Index() // Phương thức bất đồng bộ
		{
			return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
		}
        public async Task<IActionResult> ViewOrder(string Ordercode) // Phương thức bất đồng bộ
        {
			var DetailsOrder = await _dataContext.OrderDetails.Include(od =>od.Product).Where(od=>od.OrderCode == Ordercode).ToListAsync();
            return View(DetailsOrder);
        }
    }
}
