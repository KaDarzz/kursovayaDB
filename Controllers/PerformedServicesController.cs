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
    public class PerformedServicesController : Controller
    {
        private readonly HairdressingContext _context;
        private readonly CachedDataService _cachedDataService;

        public PerformedServicesController(HairdressingContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;
        }

        // GET: PerformedServices
        public async Task<IActionResult> Index(string employeeFilter, string clientFilter, string serviceFilter, int page = 1, int pageSize = 20)
        {
            var modelsQuery = _cachedDataService.GetPerformedServices();

            // Фильтрация по имени сотрудника
            if (!string.IsNullOrEmpty(employeeFilter))
            {
                modelsQuery = modelsQuery.Where(performedService =>
                    performedService.Employee != null && performedService.Employee.FullName.Contains(employeeFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Фильтрация по имени клиента
            if (!string.IsNullOrEmpty(clientFilter))
            {
                modelsQuery = modelsQuery.Where(performedService =>
                    performedService.Client != null && performedService.Client.FullName.Contains(clientFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Фильтрация по названию услуги
            if (!string.IsNullOrEmpty(serviceFilter))
            {
                modelsQuery = modelsQuery.Where(performedService =>
                    performedService.Service != null && performedService.Service.Name.Contains(serviceFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Пагинация
            int totalItems = modelsQuery.Count();
            var performedServices = modelsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передача данных во ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.EmployeeFilter = employeeFilter;
            ViewBag.ClientFilter = clientFilter;
            ViewBag.ServiceFilter = serviceFilter;

            return View(performedServices);
        }

        // GET: PerformedServices/Details/5`
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var performedService = await _context.PerformedServices
                .Include(p => p.Client)
                .Include(p => p.Employee)
                .Include(p => p.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (performedService == null)
            {
                return NotFound();
            }

            return View(performedService);
        }

        // GET: PerformedServices/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            return View();
        }


        // POST: PerformedServices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientId,ServiceId,EmployeeId,ServiceDate,Cost")] PerformedService performedService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(performedService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", performedService.ClientId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", performedService.EmployeeId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", performedService.ServiceId);
            return View(performedService);
        }


        // GET: PerformedServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var performedService = await _context.PerformedServices.FindAsync(id);
            if (performedService == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", performedService.ClientId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", performedService.EmployeeId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", performedService.ServiceId);
            return View(performedService);
        }


        // POST: PerformedServices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,ServiceId,EmployeeId,ServiceDate,Cost")] PerformedService performedService)
        {
            if (id != performedService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(performedService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerformedServiceExists(performedService.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", performedService.ClientId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", performedService.EmployeeId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", performedService.ServiceId);
            return View(performedService);
        }


        // GET: PerformedServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var performedService = await _context.PerformedServices
                .Include(p => p.Client)
                .Include(p => p.Employee)
                .Include(p => p.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (performedService == null)
            {
                return NotFound();
            }

            return View(performedService);
        }


        // POST: PerformedServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var performedService = await _context.PerformedServices.FindAsync(id);
            if (performedService != null)
            {
                _context.PerformedServices.Remove(performedService);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PerformedServiceExists(int id)
        {
            return _context.PerformedServices.Any(e => e.Id == id);
        }
    }
}
