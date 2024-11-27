using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TransportCompanyWithAuthorize.Data;
using TransportCompanyWithAuthorize.Models;
using TransportCompanyWithAuthorize.Service;

namespace TransportCompanyWithAuthorize.Controllers
{
    public class EmployeeSchedulesController : Controller
    {
        private readonly HairdressingContext _context;
        private readonly CachedDataService _cachedDataService;

        public EmployeeSchedulesController(HairdressingContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;
        }

        // GET: EmployeeSchedules
        public async Task<IActionResult> Index(string nameFilter, string date, int page = 1, int pageSize = 20)
        {
            var modelsQuery = _cachedDataService.GetEmployeeSchedules(); // Получаем записи

            // Фильтрация по имени
            if (!string.IsNullOrEmpty(nameFilter))
            {
                modelsQuery = modelsQuery.Where(employeeSchedules =>
                    employeeSchedules.Employee.FullName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Фильтрация по дате
            if (!string.IsNullOrEmpty(date))
            {
                // Попытка преобразовать строку в DateOnly
                if (DateOnly.TryParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    modelsQuery = modelsQuery.Where(employeeSchedules => employeeSchedules.WorkDate == parsedDate);
                }
            }

            // Пагинация
            int totalItems = modelsQuery.Count();
            var employeeSchedules = modelsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передаем данные во ViewBag для создания пагинации и сохранения фильтров
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.NameFilter = nameFilter;
            ViewBag.Date = date;

            return View(employeeSchedules);
        }


        // GET: EmployeeSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeSchedule = await _context.EmployeeSchedules
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeSchedule == null)
            {
                return NotFound();
            }

            return View(employeeSchedule);
        }

        // GET: EmployeeSchedules/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            return View();
        }

        // POST: EmployeeSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,WorkDate,IsWorking")] EmployeeSchedule employeeSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", employeeSchedule.EmployeeId);
            return View(employeeSchedule);
        }

        // GET: EmployeeSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeSchedule = await _context.EmployeeSchedules.FindAsync(id);
            if (employeeSchedule == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", employeeSchedule.EmployeeId);
            return View(employeeSchedule);
        }

        // POST: EmployeeSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,WorkDate,IsWorking")] EmployeeSchedule employeeSchedule)
        {
            if (id != employeeSchedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeScheduleExists(employeeSchedule.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", employeeSchedule.EmployeeId);
            return View(employeeSchedule);
        }

        // GET: EmployeeSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeSchedule = await _context.EmployeeSchedules
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeSchedule == null)
            {
                return NotFound();
            }

            return View(employeeSchedule);
        }

        // POST: EmployeeSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeSchedule = await _context.EmployeeSchedules.FindAsync(id);
            if (employeeSchedule != null)
            {
                _context.EmployeeSchedules.Remove(employeeSchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeScheduleExists(int id)
        {
            return _context.EmployeeSchedules.Any(e => e.Id == id);
        }
    }
}
