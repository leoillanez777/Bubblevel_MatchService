using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bubblevel_MatchService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Bubblevel_MatchService.Context;
using Bubblevel_MatchService.Models;
namespace Bubblevel_MatchService.Controllers;

public class InterventionController : Controller {
  private readonly ApplicationDbContext _context;
  private readonly IEmailSender _email;

  public InterventionController(ApplicationDbContext context, IEmailSender email)
  {
    _context = context;
    _email = email;
  }

  // GET: Intervention
  public async Task<IActionResult> Index(int id)
  {
    var applicationDbContext = _context.Intervention
      .Include(i => i.SupportIncident)
      .Where(i => i.SupportIncidentId == id);

    var dataSupport = await _context.SupportIncident
      .Include(c => c.Customer)
      .FirstOrDefaultAsync(s => s.Id == id);
    ViewBag.SupportId = id;
    ViewBag.CustomerName = dataSupport!.Customer!.Name;
    ViewBag.Summary = dataSupport.Summary;

    return View(await applicationDbContext.ToListAsync());
  }

  // GET: Intervention/Create
  public IActionResult Create(int supportId, string customerName)
  {
    ViewBag.SupportId = supportId;
    ViewBag.CustomerName = customerName;

    return View();
  }

  // POST: Intervention/Create
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create([Bind("Id,Description,InterventionDate,Duration,SupportIncidentId")] Intervention intervention, string customerName)
  {
    if (intervention.SupportIncidentId != 0) {
      intervention.SupportIncident = await _context.SupportIncident.FirstOrDefaultAsync(s => s.Id == intervention.SupportIncidentId);
    }

    if (ModelState.IsValid) {
      _context.Add(intervention);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index), new { id = intervention.SupportIncidentId });
    }

    ViewBag.SupportId = intervention.SupportIncidentId;
    ViewBag.CustomerName = customerName;

    return View(intervention);
  }

  // GET: Intervention/Edit/5
  public async Task<IActionResult> Edit(int? id, int supportId, string customerName)
  {
    ViewBag.SupportId = supportId;
    ViewBag.CustomerName = customerName;

    if (id == null || _context.Intervention == null) {
      return NotFound();
    }

    var intervention = await _context.Intervention.FindAsync(id);
    if (intervention == null) {
      return NotFound();
    }
    
    return View(intervention);
  }

  // POST: Intervention/Edit/5
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, [Bind("Id,Description,InterventionDate,Duration,SupportIncidentId")] Intervention intervention, string customerName)
  {
    ViewBag.SupportId = intervention.SupportIncidentId;
    ViewBag.CustomerName = customerName;

    if (id != intervention.Id) {
      return NotFound();
    }

    if (intervention.SupportIncidentId != 0) {
      intervention.SupportIncident = await _context.SupportIncident.FirstOrDefaultAsync(s => s.Id == intervention.SupportIncidentId);
    }

    if (ModelState.IsValid) {
      try {
        _context.Update(intervention);
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) {
        if (!InterventionExists(intervention.Id)) {
          return NotFound();
        }
        else {
          throw;
        }
      }
      return RedirectToAction(nameof(Index), new { id = intervention.SupportIncidentId });
    }
    
    return View(intervention);
  }

  // GET: Intervention/EditEmail5
  public async Task<IActionResult> Email(int? id, int supportId, string customerName)
  {
    ViewBag.SupportId = supportId;
    ViewBag.CustomerName = customerName;

    if (id == null || _context.Intervention == null) {
      return NotFound();
    }

    var intervention = await _context.Intervention.FindAsync(id);
    if (intervention == null) {
      return NotFound();
    }

    return View(intervention);
  }

  // POST: Intervention/Email/5
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Email(int id, [Bind("Id,Description,InterventionDate,Duration,SupportIncidentId")] Intervention intervention, string customerName)
  {
    ViewBag.SupportId = intervention.SupportIncidentId;
    ViewBag.CustomerName = customerName;

    if (id != intervention.Id) {
      return NotFound();
    }

    if (intervention.SupportIncidentId != 0) {
      intervention.SupportIncident = await _context.SupportIncident
        .Include(s => s.Customer)
        .FirstOrDefaultAsync(s => s.Id == intervention.SupportIncidentId);
    }
    Customer? customer = null;
    if (intervention.SupportIncident != null && intervention.SupportIncident.Customer != null) {
      customer = intervention.SupportIncident.Customer;
    }

    if (ModelState.IsValid && customer is not null) {
      try {

        TimeSpan time = TimeSpan.FromHours((double)intervention.Duration);
        string format = $"{time.Hours}hr {time.Minutes}min";

        await _email.SendEmailAsync(
            customer.Email,
            $"Intervention Nº {intervention.Id}",
            $"{intervention.Description} <hr /> <b>Date:</b> {intervention.InterventionDate} <hr /> <b>Duration:</b> {format}",
            customer.Name);

      }
      catch (DbUpdateConcurrencyException) {
        if (!InterventionExists(intervention.Id)) {
          return NotFound();
        }
        else {
          throw;
        }
      }
      return RedirectToAction(nameof(Index), new { id = intervention.SupportIncidentId });
    }

    return View(intervention);
  }

  // GET: Intervention/Delete/5
  public async Task<IActionResult> Delete(int? id)
  {
    if (id == null || _context.Intervention == null) {
      return NotFound();
    }

    var intervention = await _context.Intervention
        .Include(i => i.SupportIncident)
        .FirstOrDefaultAsync(m => m.Id == id);
    if (intervention == null) {
      return NotFound();
    }

    return View(intervention);
  }

  // POST: Intervention/Delete/5
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    if (_context.Intervention == null) {
      return Problem("Entity set 'ApplicationDbContext.Intervention'  is null.");
    }
    var intervention = await _context.Intervention.FindAsync(id);
    if (intervention != null) {
      _context.Intervention.Remove(intervention);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
  }

  private bool InterventionExists(int id)
  {
    return (_context.Intervention?.Any(e => e.Id == id)).GetValueOrDefault();
  }

}
