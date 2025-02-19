using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQLDASinhVien.Data; // Giả sử DbContext nằm ở đây
using WebQLDASinhVien.Models;

namespace WebQLDASinhVien.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly WebQLDASinhVienContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProjectsController(WebQLDASinhVienContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách đồ án kèm theo thông tin sinh viên và giáo viên
            var projects = _context.Projects
                                   .Include(p => p.Student)
                                   .Include(p => p.Teacher);
            return View(await projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _context.Projects
                                        .Include(p => p.Student)
                                        .Include(p => p.Teacher)
                                        .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
                return NotFound();

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            // Đưa dữ liệu vào dropdown cho sinh viên và giáo viên
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName");
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "FullName");
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project, IFormFile? fileUpload)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload file nếu có
                if (fileUpload != null && fileUpload.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Tạo tên file duy nhất để tránh trùng lặp
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.CopyToAsync(fileStream);
                    }
                    // Lưu đường dẫn tương đối để có thể hiển thị file
                    project.FilePath = "/uploads/" + uniqueFileName;
                }

                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Nếu có lỗi thì trả về view với các dropdown đã có dữ liệu
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", project.StudentId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "FullName", project.TeacherId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", project.StudentId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "FullName", project.TeacherId);
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project, IFormFile? fileUpload)
        {
            if (id != project.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu có file mới được upload thì thay thế file cũ
                    if (fileUpload != null && fileUpload.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileUpload.CopyToAsync(fileStream);
                        }
                        project.FilePath = "/uploads/" + uniqueFileName;
                    }

                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", project.StudentId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "FullName", project.TeacherId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _context.Projects
                                        .Include(p => p.Student)
                                        .Include(p => p.Teacher)
                                        .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
                return NotFound();

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                // Xóa file đồ án khỏi server nếu có
                if (!string.IsNullOrEmpty(project.FilePath))
                {
                    string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, project.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
