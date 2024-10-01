using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuochoaHuxtah.Models;
using NuochoaHuxtah.Repository;
using System.Linq.Expressions;
using System.Numerics;

namespace NuochoaHuxtah.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("Admin/User")]
	[Authorize]
	public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        public UserController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, DataContext Context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext =  Context;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = await (from u in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id
                                        select new {User = u, RoleName = r.Name}).ToListAsync();
            return View(usersWithRoles);
        }
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if(ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user,user.PasswordHash);
                if (createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email);//Tifm user dựa vào email
                    var userId = createUser.Id;
                    var role = _roleManager.FindByIdAsync(user.RoleId); //Lấy RoleId
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser,role.Result.Name); //Lấy role dựa vào name
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach(var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                }
            }
            else
            {
                TempData["error"] = "Model not ok";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Người dùng đã được xóa thành công";  
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(AppUserModel user,string id)
        {
            var existringUser = await _userManager.FindByIdAsync(id);//Lấy user dựa vào id
            if (existringUser == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //Cập nhật các thuộc tính của user(ngoại trừ pass)
                existringUser.UserName = user.UserName;
                existringUser.Email = user.Email;
                existringUser.PhoneNumber = user.PhoneNumber;
                existringUser.RoleId = user.RoleId;
                
                var updateUserResult = await _userManager.UpdateAsync(existringUser);
                if (updateUserResult.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in updateUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(existringUser);
                }
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            TempData["error"] = "Model validation failed";
            var errors = ModelState.Values.SelectMany(v=>v.Errors.Select(e=>e.ErrorMessage)).ToList();
            string errorMessage = string.Join("\n", errors);
            return View(existringUser);
            
        }

    }
}
