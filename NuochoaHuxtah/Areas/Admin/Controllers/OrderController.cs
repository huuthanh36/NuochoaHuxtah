 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuochoaHuxtah.Models;
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
		[HttpGet]
		[Route("Index")]
		public async Task<IActionResult> Index() // Phương thức bất đồng bộ
		{
			return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
		}
        [HttpGet]
        [Route("ViewOrder")]
        public async Task<IActionResult> ViewOrder(string ordercode) // Phương thức bất đồng bộ
        {
			var DetailsOrder = await _dataContext.OrderDetails.Include(od =>od.Product).Where(od=>od.OrderCode == ordercode).ToListAsync();
			
            return View(DetailsOrder);
        }
		[HttpPost]
		[Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status) // Phương thức bất đồng bộ
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
			if(order == null)
			{
				return NotFound();
			}
			order.Status = status;
			try
			{
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
			{
				return StatusCode(500, "Có lỗi trong quá trình cập nhật trạng thái");
			}
        }
        
        public async Task<IActionResult> Delete(int Id)
        {
            OrderModel order = await _dataContext.Orders.FindAsync(Id);

            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Đơn hàng đã được xóa khỏi database";
            return RedirectToAction("Index");
        }
    }
}
