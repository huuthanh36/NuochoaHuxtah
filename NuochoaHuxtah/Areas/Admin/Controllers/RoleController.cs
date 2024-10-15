using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuochoaHuxtah.Models;
using NuochoaHuxtah.Repository;
using System.Security.AccessControl;

namespace NuochoaHuxtah.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/Role")]
    [Authorize]
    public class RoleController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(DataContext context, RoleManager<IdentityRole> roleManager)
        {
            _dataContext = context;
            _roleManager = roleManager;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Roles.OrderByDescending(p => p.Id).ToListAsync());
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            TempData["success"] = "Đã thêm thành công quyền";
            return Redirect("Index");
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound();
            }
            try
            {
                await _roleManager.DeleteAsync(role);
                TempData["success"] = "Đã xóa thành công quyền";
            }catch (Exception ex)
            {
                ModelState.AddModelError("", "Gặp lỗi trong khi xóa quyền");
            }
            return Redirect("Index");
        }
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            return View(role);  
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IdentityRole model)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if(role == null)
                {
                    return NotFound();
                }
                role.Name = model.Name;
                try
                {
                    await _roleManager.UpdateAsync(role);
                    TempData["success"] = "Cập nhật thành công";
                    return RedirectToAction("Index");
                }catch (Exception ex)
                {
                    ModelState.AddModelError("", "Gặp lỗi khi cập nhật quyền");
                }
            }
            return View(model ?? new IdentityRole { Id = id}); // Trả về View nếu model hoặc id không tồn tại
        }
    }
}
