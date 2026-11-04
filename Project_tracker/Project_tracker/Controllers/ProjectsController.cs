using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_tracker.Data;
using Project_tracker.Models;
using Microsoft.AspNetCore.Authorization;

namespace Project_tracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Projects
        // Получаем список всех проектов
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.Owner)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();

            return View(projects);
        }

        // GET: /Projects/Details/1
        // Получаем один проект вместе с его задачами
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            return View(project);
        }

        // GET: /Projects/Create
        // GET: /Projects/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadOwners();
            return View();
        }

        // POST: /Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Project project)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == project.OwnerId))
            {
                ModelState.AddModelError(
                    nameof(project.OwnerId),
                    "Выберите существующего владельца проекта.");
            }

            if (ModelState.IsValid)
            {
                project.StartDate = DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc);
                if (project.EndDate.HasValue)
                    project.EndDate = DateTime.SpecifyKind(project.EndDate.Value, DateTimeKind.Utc);

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await LoadOwners(project.OwnerId);
            return View(project);
        }

        // GET: /Projects/Edit/1
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _context.Projects.FindAsync(id);

            if (project == null)
                return NotFound();

            await LoadOwners(project.OwnerId);
            return View(project);
        }

        // POST: /Projects/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id)
                return NotFound();

            if (!await _context.Users.AnyAsync(u => u.Id == project.OwnerId))
            {
                ModelState.AddModelError(
                    nameof(project.OwnerId),
                    "Выберите существующего владельца проекта.");
            }

            if (ModelState.IsValid)
            {
                project.StartDate = DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc);
                if (project.EndDate.HasValue)
                    project.EndDate = DateTime.SpecifyKind(project.EndDate.Value, DateTimeKind.Utc);

                _context.Update(project);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await LoadOwners(project.OwnerId);
            return View(project);
        }

        // GET: /Projects/Delete/1
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            return View(project);
        }

        // POST: /Projects/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Заполняем выпадающий список владельцев
        private async Task LoadOwners(int? selectedOwnerId = null)
        {
            var users = await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();

            ViewBag.OwnerId = new SelectList(
                users,
                nameof(Project_tracker.Models.User.Id),
                nameof(Project_tracker.Models.User.Username),
                selectedOwnerId);
        }
    }
}
