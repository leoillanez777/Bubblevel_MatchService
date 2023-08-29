using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bubblevel_MatchService.Context;
using Bubblevel_MatchService.Models;
using Bubblevel_MatchService.Services.Interfaces;

namespace Bubblevel_MatchService.Controllers;

public class SupportIncidentController : Controller {

  private readonly ApplicationDbContext _context;
  private readonly IEmailSender _email;

  public SupportIncidentController(ApplicationDbContext context, IEmailSender email)
  {
    _context = context;
    _email = email;
  }

  // GET: SupportIncident
  public async Task<IActionResult> Index(int? page)
  {
    int pageSize = 10;
    int pageNumber = page ?? 1;
    var applicationDbContext = _context.SupportIncident.Include(s => s.Customer).Include(s => s.Project);
    
    return View(await applicationDbContext.ToPagedListAsync(pageNumber, pageSize));
  }

  // GET: SupportIncident/Details/5
  public async Task<IActionResult> Details(int? id)
  {
    if (id == null || _context.SupportIncident == null) {
      return NotFound();
    }

    var supportIncident = await _context.SupportIncident
        .Include(s => s.Customer)
        .Include(s => s.Project)
        .FirstOrDefaultAsync(m => m.Id == id);
    if (supportIncident == null) {
      return NotFound();
    }

    return View(supportIncident);
  }

  // GET: SupportIncident/Create
  public IActionResult Create()
  {
    return View();
  }

  // POST: SupportIncident/Create
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create([Bind("Id,Summary,IsResolved,TotalDebited,CustomerId,ProjectId")] SupportIncident supportIncident)
  {
    if (supportIncident.CustomerId != 0) {
      supportIncident.Customer = await _context.Customer.FindAsync(supportIncident.CustomerId);
    }
    if (supportIncident.ProjectId != 0) {
      supportIncident.Project = await _context.Project.FindAsync(supportIncident.ProjectId);
    }
    if (ModelState.IsValid && supportIncident.Customer != null && supportIncident.Project != null) {
      // 1) Enviar email a cliente si tiene intervenciones...
      // 2) verificar si tiene plan activo de soporte.
      //  2a) NO: Enviar email a finances.
      //  2b) SI: 
      _context.Add(supportIncident);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    return View(supportIncident);
  }

  // GET: SupportIncident/Edit/5
  public async Task<IActionResult> Edit(int? id)
  {
    if (id == null || _context.SupportIncident == null) {
      return NotFound();
    }

    var supportIncident = await _context.SupportIncident.FindAsync(id);
    if (supportIncident == null) {
      return NotFound();
    }
    ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "Email", supportIncident.CustomerId);
    ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", supportIncident.ProjectId);
    return View(supportIncident);
  }

  // POST: SupportIncident/Edit/5
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, [Bind("Id,Summary,IsResolved,TotalDebited,CustomerId,ProjectId")] SupportIncident supportIncident)
  {
    if (id != supportIncident.Id) {
      return NotFound();
    }

    if (ModelState.IsValid) {
      try {
        _context.Update(supportIncident);
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) {
        if (!SupportIncidentExists(supportIncident.Id)) {
          return NotFound();
        }
        else {
          throw;
        }
      }
      return RedirectToAction(nameof(Index));
    }
    ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "Email", supportIncident.CustomerId);
    ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", supportIncident.ProjectId);
    return View(supportIncident);
  }

  // GET: SupportIncident/Delete/5
  public async Task<IActionResult> Delete(int? id)
  {
    if (id == null || _context.SupportIncident == null) {
      return NotFound();
    }

    var supportIncident = await _context.SupportIncident
        .Include(s => s.Customer)
        .Include(s => s.Project)
        .FirstOrDefaultAsync(m => m.Id == id);
    if (supportIncident == null) {
      return NotFound();
    }

    return View(supportIncident);
  }

  // POST: SupportIncident/Delete/5
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    if (_context.SupportIncident == null) {
      return Problem("Entity set 'ApplicationDbContext.SupportIncident'  is null.");
    }
    var supportIncident = await _context.SupportIncident.FindAsync(id);
    if (supportIncident != null) {
      _context.SupportIncident.Remove(supportIncident);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
  }

  private bool SupportIncidentExists(int id)
  {
    return (_context.SupportIncident?.Any(e => e.Id == id)).GetValueOrDefault();
  }
}