using System;
using System.Collections.Generic;
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
    public class ServiceTypesController : Controller
    {
        private readonly HairdressingContext _context;
        private readonly CachedDataService _cachedDataService;

        public ServiceTypesController(HairdressingContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;
        }

        // GET: ServiceTypes
        public async Task<IActionResult> Index(string serviceName, string description, int page = 1, int pageSize = 20)
        {
            var modelsQuery = _cachedDataService.GetServiceTypes();

            // Фильтрация по имени сервиса
            if (!string.IsNullOrEmpty(serviceName))
            {
                modelsQuery = modelsQuery.Where(s =>
                    s.Name.Contains(serviceName, StringComparison.OrdinalIgnoreCase));
            }

            // Фильтрация по описанию
            if (!string.IsNullOrEmpty(description))
            {
                modelsQuery = modelsQuery.Where(s =>
                    s.Description.Contains(description, StringComparison.OrdinalIgnoreCase));
            }

            // Пагинация
            int totalItems = modelsQuery.Count();
            var serviceTypes = modelsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передача данных во ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.ServiceName = serviceName;
            ViewBag.Description = description;

            return View(serviceTypes);
        }

        // GET: ServiceTypes/Details/5
        public async Task<IActionResult> Details(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceType = await _context.ServiceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceType == null)
            {
                return NotFound();
            }

            return View(serviceType);
        }

        // GET: ServiceTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiceTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] ServiceType serviceType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceType);
        }

        // GET: ServiceTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceType = await _context.ServiceTypes.FindAsync(id);
            if (serviceType == null)
            {
                return NotFound();
            }
            return View(serviceType);
        }

        // POST: ServiceTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] ServiceType serviceType)
        {
            if (id != serviceType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceTypeExists(serviceType.Id))
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
            return View(serviceType);
        }

        // GET: ServiceTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceType = await _context.ServiceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceType == null)
            {
                return NotFound();
            }

            return View(serviceType);
        }

        // POST: ServiceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceType = await _context.ServiceTypes.FindAsync(id);
            if (serviceType != null)
            {
                _context.ServiceTypes.Remove(serviceType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceTypeExists(int id)
        {
            return _context.ServiceTypes.Any(e => e.Id == id);
        }
    }
}
