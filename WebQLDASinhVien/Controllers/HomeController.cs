using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebQLDASinhVien.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
          var fullName = User.Identity.Name; 
          ViewData["FullName"] = fullName;

            return View();
        }
    }
}
