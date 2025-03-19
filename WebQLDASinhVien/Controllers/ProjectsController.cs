using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQLDASinhVien.Data;
using WebQLDASinhVien.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebQLDASinhVien.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class ProjectsController : Controller
    {
        private readonly WebQLDASinhVienContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProjectsController(WebQLDASinhVienContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Hiển thị danh sách đồ án của tất cả sinh viên, hỗ trợ tìm kiếm theo tiêu đề hoặc tên sinh viên
        public async Task<IActionResult> Index(string searchString)
        {
            var projects = _context.Projects.Include(p => p.Student).Include(p => p.Teacher).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(p => p.Title.Contains(searchString)
                                             || (p.Student != null && p.Student.FullName.Contains(searchString)));
            }
            return View(await projects.ToListAsync());
        }

        // GET: Tạo đồ án mới (đăng ký đồ án cho sinh viên)
        public IActionResult Create()
        {
            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName");
            ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "FullName");
            return View();
        }

        // POST: Tạo đồ án mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project, IFormFile? fileUpload, string? googleDocLink)
        {
            if (ModelState.IsValid)
            {
                // Xử lý file upload nếu có file được chọn
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

                // Khởi tạo tiến độ mặc định và phản hồi mặc định
                project.Progress = 0;
                project.Feedback = string.Empty;

                _context.Add(project);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đồ án đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName", project.StudentId);
            ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "FullName", project.TeacherId);
            return View(project);
        }

        // GET: Xem chi tiết đồ án
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _context.Projects
                .Include(p => p.Student)
                .Include(p => p.Teacher)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            return View(project);
        }

        // GET: Chỉnh sửa đồ án (cho Admin và Giáo viên)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName", project.StudentId);
            ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "FullName", project.TeacherId);
            return View(project);
        }

        // POST: Chỉnh sửa đồ án
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
                    // Xử lý cập nhật file nếu có file mới được upload hoặc link Google Doc mới
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

                    _context.Update(project);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đồ án đã được cập nhật!";
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
            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName", project.StudentId);
            ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "FullName", project.TeacherId);
            return View(project);
        }

        // GET: Xóa đồ án
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _context.Projects
                .Include(p => p.Student)
                .Include(p => p.Teacher)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
                return NotFound();

            return View(project);
        }

        // POST: Xóa đồ án
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đồ án đã được xóa!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
