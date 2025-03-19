using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQLDASinhVien.Data;
using WebQLDASinhVien.Models;

namespace WebQLDASinhVien.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly WebQLDASinhVienContext _context;

        public SchedulesController(WebQLDASinhVienContext context)
        {
            _context = context;
        }

        // GET: Schedules
        public async Task<IActionResult> Index()
        {
            var schedules = _context.Schedules.Include(s => s.Project);
            return View(await schedules.ToListAsync());
        }

        // GET: Schedules/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Projects, "Id", "Title");
            return View();
        }

        // POST: Schedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xung đột lịch (ví dụ: cùng ngày, cùng phòng)
                var conflict = _context.Schedules.Any(s => s.ScheduledDate == schedule.ScheduledDate && s.Venue == schedule.Venue);
                if (conflict)
                {
                    ModelState.AddModelError("", "Lịch chấm bị trùng tại cùng địa điểm.");
                    ViewData["ProjectId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Projects, "Id", "Title", schedule.ProjectId);
                    return View(schedule);
                }
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Projects, "Id", "Title", schedule.ProjectId);
            return View(schedule);
        }

        // GET: Schedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();
            ViewData["ProjectId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Projects, "Id", "Title", schedule.ProjectId);
            return View(schedule);
        }

        // POST: Schedules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Schedule schedule)
        {
            if (id != schedule.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Projects, "Id", "Title", schedule.ProjectId);
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var schedule = await _context.Schedules.Include(s => s.Project).FirstOrDefaultAsync(s => s.Id == id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.Id == id);
        }
    }
}
