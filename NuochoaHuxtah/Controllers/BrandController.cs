using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuochoaHuxtah.Models;
using NuochoaHuxtah.Repository;

namespace NuochoaHuxtah.Controllers
{
	public class BrandController : Controller
	{
		private readonly DataContext _dataContext;
		public BrandController(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IActionResult> Index(string Slug = "")
		{
			BrandModel brand = _dataContext.Brands.Where(p => p.Slug == Slug).FirstOrDefault();
			if (brand == null) // Nếu brand không có thì trả về trang index
			{
				return RedirectToAction("Index");
			}
			var productsByCategory = _dataContext.Products.Where(p => p.BrandId == brand.Id); // Điều kiện brandid phải bằng brand.id
			return View(await productsByCategory.OrderByDescending(p => p.Id).ToListAsync());// OrderByDescending() : Giống LIFO(stack)
		}
	}
}
