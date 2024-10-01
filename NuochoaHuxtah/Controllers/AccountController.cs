using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuochoaHuxtah.Areas.Admin.Repository;
using NuochoaHuxtah.Models;
using NuochoaHuxtah.Models.ViewModels;

namespace NuochoaHuxtah.Controllers
{
	public class AccountController : Controller
	{
		//Đăng ký dịch vụ đăng nhập và quản lý user của identity
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IEmailSender emailSender)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_emailSender = emailSender;
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
		[HttpPost]
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
