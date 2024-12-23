using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQLDASinhVien.Data;
using WebQLDASinhVien.Models;

namespace WebQLDASinhVien.Controllers
{
    public class StudentsController : Controller
    {
        private readonly WebQLDASinhVienContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public StudentsController(WebQLDASinhVienContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Students
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 10; // Số lượng sinh viên mỗi trang
            var students = from s in _context.Students select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.FullName.Contains(searchString) ||
                                               s.StudentCode.Contains(searchString) ||
                                               s.Email.Contains(searchString));
            }

            var paginatedList = await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber, pageSize);
            return View(paginatedList);
        }


        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                // Thêm học sinh vào database
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                // Tạo tài khoản cho học sinh
                var user = new ApplicationUser
                {
                    UserName = student.Email,
                    Email = student.Email,
                    FullName = student.FullName
                };

                var result = await _userManager.CreateAsync(user, student.StudentCode); // Mật khẩu là mã sinh viên

                if (result.Succeeded)
                {
                    // Kiểm tra nếu role "Student" đã tồn tại, nếu chưa thì tạo mới
                    if (!await _roleManager.RoleExistsAsync("Student"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Student"));
                    }

                    // Gán role "Student" cho tài khoản mới tạo
                    await _userManager.AddToRoleAsync(user, "Student");

                    TempData["Success"] = "Thêm học sinh và tạo tài khoản thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Xóa học sinh nếu tạo tài khoản thất bại
                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();

                    ModelState.AddModelError("", "Không thể tạo tài khoản cho học sinh.");
                }
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentCode,FullName,DateOfBirth,Address,Email,PhoneNumber")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Lấy thông tin sinh viên cần xóa
            var student = await _context.Students.FindAsync(id);

            if (student != null)
            {
                // Tìm tài khoản liên kết qua email
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == student.Email);

                // Nếu tài khoản tồn tại, tiến hành xóa
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        // Nếu xóa tài khoản thất bại, thông báo lỗi
                        ModelState.AddModelError("", "Không thể xóa tài khoản của sinh viên.");
                        return View(student);
                    }
                }

                // Xóa sinh viên khỏi cơ sở dữ liệu
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
