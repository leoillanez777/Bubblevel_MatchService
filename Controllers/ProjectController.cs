using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bubblevel_MatchService.Context;
using Bubblevel_MatchService.Models;
using Microsoft.AspNetCore.Authorization;

namespace Bubblevel_MatchService.Controllers;

public class ProjectController : Controller {
  private readonly ApplicationDbContext _context;

  public ProjectController(ApplicationDbContext context)
  {
    _context = context;
  }

  // GET: Project
  [Authorize(Roles = "SuperAdmin,Admin,Project,ProjectAdd,ProjectEdit,ProjectDelete")]
  public async Task<IActionResult> Index()
  {
    return _context.Project != null ?
                View(await _context.Project.ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.Project'  is null.");
  }

  // GET: Json data
  [AllowAnonymous]
  public async Task<IActionResult> GetProject(string? filter)
  {
    filter = filter?.ToLower();

    IQueryable<Project> query = _context.Project;

    if (!string.IsNullOrEmpty(filter)) {
      query = query.Where(c => c.Name.ToLower().Contains(filter));
    }
    else {
      query = query.Take(10);
    }

    var filteredCustomers = await query.ToListAsync();

    return Json(filteredCustomers);
  }

  [AllowAnonymous]
  public IActionResult GetTime(int filter)
  {
    int min = filter - 100;
    if (min < 0) {
      min = 0;
    }
    int max = Math.Min(int.MaxValue, filter + 50);
    var timeItems = from hours in Enumerable.Range(min, max)
                    from minutes in new[] { 0, 15, 30, 45 }
                    select new TimeItem {
                      State = Tools.BuildStateText(hours, minutes),
                      Abbreviation = Tools.BuildAbbreviation(hours, minutes)
                    };

    return Json(timeItems);
  }

  // GET: Project/Details/5
  [Authorize(Roles = "SuperAdmin,Admin,Project,ProjectAdd,ProjectEdit,ProjectDelete")]
  public async Task<IActionResult> Details(int? id)
  {
    if (id == null || _context.Project == null) {
      return NotFound();
    }

    var project = await _context.Project
        .FirstOrDefaultAsync(m => m.Id == id);
    if (project == null) {
      return NotFound();
    }

    return View(project);
  }

  // GET: Project/Create
  [Authorize(Roles = "SuperAdmin,Admin,Project,ProjectAdd")]
  public IActionResult Create()
  {
    return View();
  }

  // POST: Project/Create
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(Roles = "SuperAdmin,Admin,Project,ProjectAdd")]
  public async Task<IActionResult> Create([Bind("Id,Name,IntialDate,Duration,Closed")] Project project)
  {
    if (ModelState.IsValid) {
      _context.Add(project);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    return View(project);
  }

  // GET: Project/Edit/5
  [Authorize(Roles = "SuperAdmin,Admin,Project,ProjectEdit")]
  public async Task<IActionResult> Edit(int? id)
  {
    if (id == null || _context.Project == null) {
      return NotFound();
    }

    var project = await _context.Project.FindAsync(id);
    if (project == null) {
      return NotFound();
    }

    return View(project);
  }

  // POST: Project/Edit/5
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(Roles = "SuperAdmin,Admin,Project,ProjectEdit")]
  public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IntialDate,Duration,Closed")] Project project)
  {
    if (id != project.Id) {
      return NotFound();
    }

    if (ModelState.IsValid) {
      try {
        _context.Update(project);
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) {
        if (!ProjectExists(project.Id)) {
          return NotFound();
        }
        else {
          throw;
        }
      }
      return RedirectToAction(nameof(Index));
    }

    return View(project);
  }

  // GET: Project/Delete/5
  [Authorize(Roles = "SuperAdmin,Admin,Project,ProjectDelete")]
  public async Task<IActionResult> Delete(int? id)
  {
    if (id == null || _context.Project == null) {
      return NotFound();
    }

    var project = await _context.Project
        .FirstOrDefaultAsync(m => m.Id == id);
    if (project == null) {
      return NotFound();
    }

    return View(project);
  }

  // POST: Project/Delete/5
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  [Authorize(Roles = "SuperAdmin,Admin,Project,ProjectDelete")]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    if (_context.Project == null) {
      return Problem("Entity set 'ApplicationDbContext.Project'  is null.");
    }
    var project = await _context.Project.FindAsync(id);
    if (project != null) {
      _context.Project.Remove(project);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
  }

  private bool ProjectExists(int id)
  {
    return (_context.Project?.Any(e => e.Id == id)).GetValueOrDefault();
  }

}
