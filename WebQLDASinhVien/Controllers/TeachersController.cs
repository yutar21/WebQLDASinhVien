using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQLDASinhVien.Data;
using WebQLDASinhVien.Models;

namespace WebQLDASinhVien.Controllers
{
    public class TeachersController : Controller
    {
        private readonly WebQLDASinhVienContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TeachersController(WebQLDASinhVienContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Teachers
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 10;
            var teachers = from t in _context.Teachers select t;

            if (!string.IsNullOrEmpty(searchString))
            {
                teachers = teachers.Where(t => t.FullName.Contains(searchString) ||
                                               t.Email.Contains(searchString) ||
                                               t.Department.Contains(searchString));
            }

            var paginatedList = await PaginatedList<Teacher>.CreateAsync(teachers.AsNoTracking(), pageNumber, pageSize);
            return View(paginatedList);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                // Add teacher to database
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();

                // Create account for teacher
                var user = new ApplicationUser
                {
                    UserName = teacher.Email,
                    Email = teacher.Email,
                    FullName = teacher.FullName
                };

                var result = await _userManager.CreateAsync(user, teacher.Email); // Default password is the email

                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Teacher"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Teacher"));
                    }

                    await _userManager.AddToRoleAsync(user, "Teacher");

                    TempData["Success"] = "Thêm giáo viên và tạo tài khoản thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    _context.Teachers.Remove(teacher);
                    await _context.SaveChangesAsync();

                    ModelState.AddModelError("", "Không thể tạo tài khoản cho giáo viên.");
                }
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Department,PhoneNumber,DateOfBirth,Address")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher != null)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == teacher.Email);

                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Không thể xóa tài khoản của giáo viên.");
                        return View(teacher);
                    }
                }

                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
