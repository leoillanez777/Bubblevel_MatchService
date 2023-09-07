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
    var applicationDbContext = _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Comments)
      .Where(s => s.State.Equals(State.Pending) || s.State.Equals(State.Rejected));

    return View(await applicationDbContext.ToPagedListAsync(pageNumber, pageSize));
  }

  // GET: SupportIncident/ListInProgress
  public async Task<IActionResult> ListInProgress(int? page)
  {
    int pageSize = 10;
    int pageNumber = page ?? 1;
    var applicationDbContext = _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Comments)
      .Include(s => s.Project)
      .Where(s => s.State == State.InProgress);

    return View(await applicationDbContext.ToPagedListAsync(pageNumber, pageSize));
  }

  // GET: SupportIncident/ListAwaiting
  public async Task<IActionResult> ListAwaiting(int? page)
  {
    int pageSize = 10;
    int pageNumber = page ?? 1;
    var applicationDbContext = _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Comments)
      .Include(s => s.Project)
      .Where(s => s.State == State.Awaiting);

    return View(await applicationDbContext.ToPagedListAsync(pageNumber, pageSize));
  }

  // GET: SupportIncident/Create
  public IActionResult Create(string sourceView)
  {
    ViewBag.SourceView = sourceView;
    return View();
  }

  // POST: SupportIncident/Create
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create([Bind("Id,Summary,CustomerId")] SupportIncident supportIncident)
  {
    var url = $"{Request.Scheme}://{Request.Host}";
    if (supportIncident.CustomerId != 0) {
      supportIncident.Customer = await _context.Customer.FindAsync(supportIncident.CustomerId);
    }
    if (ModelState.IsValid) {
      using var dbContextTransaction = _context.Database.BeginTransaction();
      try {
        supportIncident.State = State.Pending;
        _context.Add(supportIncident);
        await _context.SaveChangesAsync();
        // TODO: generate hash and save to db.
        await _email.SendEmailAsync(
          supportIncident.Customer!.Email,
          "Pending Support Launch",
          supportIncident.Summary,
          supportIncident.State,
          supportIncident.Customer.Name);
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

  // GET: SupportIncident/Email
  public async Task<IActionResult> Email(int? id)
  {
    if (id == null || _context.SupportIncident == null) {
      return NotFound();
    }

    var appSupportIncident = _context.SupportIncident.Include(s => s.Customer);
    var supportIncident = await appSupportIncident.FirstOrDefaultAsync(s => s.Id == id);
    if (supportIncident == null) {
      return NotFound();
    }
    return View(supportIncident);
  }

  // POST: SupportIncident/Email
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Email([Bind("Id,Summary,CustomerId,State")] SupportIncident supportIncident)
  {
    var url = $"{Request.Scheme}://{Request.Host}";
    if (supportIncident.CustomerId != 0) {
      supportIncident.Customer = await _context.Customer.FindAsync(supportIncident.CustomerId);
    }
    if (ModelState.IsValid) {
      using var dbContextTransaction = _context.Database.BeginTransaction();
      try {
        _context.Update(supportIncident);
        await _context.SaveChangesAsync();
        // TODO: generate hash and save to db.
        await _email.SendEmailAsync(
          supportIncident.Customer!.Email,
          "Pending Support Launch",
          supportIncident.Summary,
          supportIncident.State,
          supportIncident.Customer.Name);
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

  public async Task<IActionResult> Approval(int? id)
  {
    if (id == null || _context.SupportIncident == null) {
      return NotFound();
    }

    var supportincident = await _context.SupportIncident
      .Include(s => s.Customer)
      .FirstOrDefaultAsync(m => m.Id == id);
    if (supportincident == null) {
      return NotFound();
    }

    return View(supportincident);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Approval(int id, [Bind("Id,Summary,CustomerId")] SupportIncident supportIncident, string state)
  {
    if (id != supportIncident.Id) {
      return NotFound();
    }

    supportIncident.Customer = await _context.Customer.FindAsync(supportIncident.CustomerId);

    if (supportIncident.Customer == null) {
      return NotFound("The customer does not exist");
    }

    bool hasActiveSupportPlan = supportIncident.Customer.HasActiveSupportPlan;
    supportIncident.State = state switch
    {
      "approval" => hasActiveSupportPlan ? State.InProgress : State.Awaiting,
      "rejected" => State.Rejected,
      _ => State.Pending,
    };

    if (ModelState.IsValid) {
      using var dbContextTransaction = _context.Database.BeginTransaction();
      try {
        _context.Update(supportIncident);
        await _context.SaveChangesAsync();

        // TODO: generate hash and save to db.
        var nameState = Extensions.EnumExtensions.GetDisplayName(supportIncident.State);
        // Send finance email only if you do not have active support
        if (supportIncident.State == State.Awaiting) {
          await _email.SendEmailAsync(
          supportIncident.Customer!.Email,
          nameState,
          supportIncident.Summary,
          supportIncident.State,
          supportIncident.Customer.Name
          );
        }

        await dbContextTransaction.CommitAsync();
      }
      catch (DbUpdateConcurrencyException ex) {
        await dbContextTransaction.RollbackAsync();
        if (!SupportIncidentExists(supportIncident.Id)) {
          return NotFound(ex.Message);
        }
        else {
          throw;
        }
      }
      return RedirectToAction(nameof(Index));
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