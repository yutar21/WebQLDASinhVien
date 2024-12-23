using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebQLDASinhVien.Models;

namespace WebQLDASinhVien.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Hiển thị form đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["Success"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chủ
            }
            else
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
            }

            return View(model);
        }

        // Đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "Bạn đã đăng xuất.";
            return RedirectToAction("Login");
        }
    }
}
