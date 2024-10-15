using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuochoaHuxtah.Areas.Admin.Repository;
using NuochoaHuxtah.Models;
using NuochoaHuxtah.Models.ViewModels;
using NuochoaHuxtah.Repository;
using System.Security.Claims;

namespace NuochoaHuxtah.Controllers
{
	public class AccountController : Controller
	{
		//Đăng ký dịch vụ đăng nhập và quản lý user của identity
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly DataContext _dataContext;
        public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IEmailSender emailSender, DataContext context)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_emailSender = emailSender;
			_dataContext = context;
		}
		public IActionResult Login(string retunrUrl)
		{

			return View(new LoginViewModel { ReturnUrl = retunrUrl });
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
				if (result.Succeeded)
				{
					return Redirect(loginVM.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", "Username hoặc password không đúng");
			}
			return View(loginVM);
		}
		
		public IActionResult Create()
		{
			return View();
		}
        public async Task<IActionResult> History()
        {
            if (!(bool)User.Identity?.IsAuthenticated)
            {
                // User is not logged in, redirect to login
                return RedirectToAction("Login", "Account"); // Replace "Account" with your controller name
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var Orders = await _dataContext.Orders
                .Where(od => od.UserName == userEmail)
                .OrderByDescending(od => od.Id)
                .ToListAsync();

            ViewBag.UserEmail = userEmail;
            return View(Orders);
        }
        public async Task<IActionResult> CancelOrder(string ordercode)
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                // User chưa login-> login
                return RedirectToAction("Login", "Account");
            }
            try
            {
                var order = await _dataContext.Orders.Where(o => o.OrderCode == ordercode).FirstAsync();
                order.Status = 3;
                _dataContext.Update(order);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi khi hủy đơn hàng.");
            }

            return RedirectToAction("History", "Account");
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(UserModel user)
		{
			if (ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
				IdentityResult result = await _userManager.CreateAsync(newUser,user.Password);
				if (result.Succeeded)
				{
					TempData["success"] = "Tạo tài khoản thành công";
                    var receiver = user.Email;
                    var subject = "Tạo tài khoản thành công";
                    var message = "Đặt hàng thành công, chúc quý khách có trải nghiệm mua sắm tuyệt vời";
                    await _emailSender.SendEmailAsync(receiver, subject, message);
                    return Redirect("/account/login");
				}
				foreach(IdentityError error in result.Errors)
				{
					ModelState.AddModelError("",error.Description);
				}
			}
			return View(user);
		}
		public async Task<RedirectResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();

			return Redirect(returnUrl);
		}

	}
}
