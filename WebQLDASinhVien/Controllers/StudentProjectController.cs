using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQLDASinhVien.Data;
using WebQLDASinhVien.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebQLDASinhVien.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentProjectController : Controller
    {
        private readonly WebQLDASinhVienContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentProjectController(WebQLDASinhVienContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Hiển thị đồ án của sinh viên (chỉ riêng sinh viên đó)
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            // Giả sử bạn có cách mapping ApplicationUser.Id và Student.Id
            // Ở đây, chuyển sang dạng string so sánh
            var project = await _context.Projects
                .Include(p => p.Teacher)
                .FirstOrDefaultAsync(p => p.StudentId.ToString() == user.Id);
            return View(project);
        }

        // GET: Sinh viên cập nhật đồ án của mình (tiến độ, phản hồi, file)
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.StudentId.ToString() == user.Id);
            if (project == null)
                return NotFound();
            return View(project);
        }

        // POST: Sinh viên cập nhật đồ án
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project, IFormFile? fileUpload, string? googleDocLink)
        {
            if (id != project.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu có file mới được upload
                    if (fileUpload != null && fileUpload.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileUpload.CopyToAsync(stream);
                        }
                        project.FilePath = "/uploads/" + uniqueFileName;
                    }
                    else if (!string.IsNullOrEmpty(googleDocLink))
                    {
                        project.FilePath = googleDocLink;
                    }

                    // Sinh viên có thể cập nhật tiến độ và phản hồi của mình
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đồ án của bạn đã được cập nhật!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Projects.Any(e => e.Id == project.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Xem chi tiết đồ án của sinh viên
        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);
            var project = await _context.Projects
                .Include(p => p.Teacher)
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.StudentId.ToString() == user.Id);
            if (project == null)
                return NotFound();
            return View(project);
        }
    }
}
