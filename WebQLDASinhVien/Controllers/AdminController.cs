using Microsoft.AspNetCore.Mvc;

namespace WebQLDASinhVien.Controllers
{
    using global::WebQLDASinhVien.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    namespace WebQLDASinhVien.Controllers
    {
        [Authorize(Roles = "Admin")] // Chỉ Admin mới được truy cập
        public class AdminController : Controller
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public AdminController(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            // GET: Tạo trang để thêm người dùng
            public IActionResult Index()
            {
                return View();
            }

            // GET: Tạo trang để thêm người dùng
            public IActionResult CreateUser()
            {
                return View();
            }

            // POST: Thêm người dùng mới
            [HttpPost]
            public async Task<IActionResult> CreateUser(string fullName, string email, string password, string role)
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = fullName
                    };

                    // Tạo người dùng mới
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        // Thêm người dùng vào vai trò cụ thể
                        var roleResult = await _userManager.AddToRoleAsync(user, role);

                        if (roleResult.Succeeded)
                        {
                            TempData["Success"] = "Tài khoản đã được tạo thành công!";
                            return RedirectToAction("CreateUser");
                        }
                        else
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                return View();
            }

        }
    }

}
