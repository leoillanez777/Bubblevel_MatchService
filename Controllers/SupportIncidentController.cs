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
    var applicationDbContext = _context.SupportIncident.Include(s => s.Customer).Include(s => s.Comments);
    
    return View(await applicationDbContext.ToPagedListAsync(pageNumber, pageSize));
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
  public async Task<IActionResult> Create([Bind("Id,Summary,CustomerId")] SupportIncident supportIncident)
  {
    if (supportIncident.CustomerId != 0) {
      supportIncident.Customer = await _context.Customer.FindAsync(supportIncident.CustomerId);
    }
    if (ModelState.IsValid) {
      using var dbContextTransaction = _context.Database.BeginTransaction();
      try {
        supportIncident.State = "Pending approval";
        _context.Add(supportIncident);
        await _context.SaveChangesAsync();
        await _email.SendEmailAsync(
          supportIncident.Customer!.Email,
          "Pending Support Launch", supportIncident.Summary);

        await dbContextTransaction.CommitAsync();

        return RedirectToAction(nameof(Index));
      }
      catch (Exception ex) {
        await dbContextTransaction.RollbackAsync();
        // TODO: Missing an exception middleware.
        throw new Exception(ex.Message, ex);
      }
      
    }

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