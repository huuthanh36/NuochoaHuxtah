using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuochoaHuxtah.Areas.Admin.Repository;
using NuochoaHuxtah.Models;
using NuochoaHuxtah.Models.ViewModels;
using NuochoaHuxtah.Repository;
using System.Security.Claims;

namespace NuochoaHuxtah.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IEmailSender _emailSender;
		public CheckoutController(DataContext _context, IEmailSender emailSender)
		{
			_dataContext = _context;
			_emailSender = emailSender;


		}
		public async Task<IActionResult> Checkout()
		{

			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if(userEmail == null)
			{
				return RedirectToAction("Login","Account");
			}
			else
			{
				//Tạo một kiểu random - mã đơn hàng(chuỗi) 
				var ordercode = Guid.NewGuid().ToString();
				var orderItem = new OrderModel();
				orderItem.OrderCode = ordercode;
				orderItem.UserName = userEmail;
				orderItem.Status = 1;
				orderItem.CreatedDate = DateTime.Now;
				_dataContext.Add(orderItem);
				_dataContext.SaveChanges();
				List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
				foreach(var cart in cartItems)
				{
					var orderdetails = new OrderDetails();
					orderdetails.UserName = userEmail;
					orderdetails.OrderCode = ordercode;
					orderdetails.ProductId = cart.ProductId;
					orderdetails.Price = cart.Price;
					orderdetails.Quantity = cart.Quantity;
					_dataContext.Add(orderdetails);
					_dataContext.SaveChanges();
				}
				HttpContext.Session.Remove("Cart");
				// Send maill
				var receiver = userEmail;
				var subject = "Đặt hàng thành công";
				var message = "Đặt hàng thành công, chúc quý khách có trải nghiệm mua sắm tuyệt vời";
				await _emailSender.SendEmailAsync(receiver, subject, message);
				TempData["success"] = "Thanh toán thành công, vui lòng chờ duyệt đơn hàng";
				return RedirectToAction("Index", "Cart");
			}
			return View();
		}
	}
}
