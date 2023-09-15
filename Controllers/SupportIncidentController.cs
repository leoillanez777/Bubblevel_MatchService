using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bubblevel_MatchService.Context;
using Bubblevel_MatchService.Models;
using Bubblevel_MatchService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Bubblevel_MatchService.Controllers;

public class SupportIncidentController : Controller {

  private readonly ApplicationDbContext _context;
  private readonly IEmailSender _email;
  private readonly IWebHostEnvironment _env;

  public SupportIncidentController(ApplicationDbContext context, IEmailSender email, IWebHostEnvironment env)
  {
    _context = context;
    _email = email;
    _env = env;
  }

  // GET: SupportIncident
  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Launch,LaunchAdd,LaunchEdit,LaunchDelete")]
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
  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "InProgress,InProgressAdd,InProgressEdit,InProgressDelete")]
  public async Task<IActionResult> ListInProgress(int? page)
  {
    int pageSize = 10;
    int pageNumber = page ?? 1;
    var applicationDbContext = _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Comments)
      .Include(s => s.Project)
      .Include(s => s.Interventions)
      .Where(s => s.State == State.InProgress);

    return View(await applicationDbContext.ToPagedListAsync(pageNumber, pageSize));
  }

  // GET: SupportIncident/AwaitingList
  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Awaiting,AwaitingAdd,AwaitingEdit,AwaitingDelete")]
  public async Task<IActionResult> AwaitingList(int? page)
  {
    int pageSize = 10;
    int pageNumber = page ?? 1;
    var applicationDbContext = _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Comments)
      .Include(s => s.Project)
      .Where(s => s.State.Equals(State.Awaiting) || s.State.Equals(State.OnHold));

    return View(await applicationDbContext.ToPagedListAsync(pageNumber, pageSize));
  }

  // GET: SupportIncident/SolvedList
  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Solved,SolvedClose,SolvedReOpen,SolvedDelete")]
  public async Task<IActionResult> SolvedList(int? page)
  {
    int pageSize = 10;
    int pageNumber = page ?? 1;
    var applicationDbContext = _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Comments)
      .Include(s => s.Project)
      .Where(s => s.State == State.Solved);

    return View(await applicationDbContext.ToPagedListAsync(pageNumber, pageSize));
  }

  // GET: SupportIncident/Create
  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Launch,LaunchAdd")]
  public IActionResult Create(string sourceView)
  {
    ViewBag.SourceView = sourceView;
    CreateViewBagForDevOrProd();

    return View();
  }

  // POST: SupportIncident/Create
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Launch,LaunchAdd")]
  public async Task<IActionResult> Create([Bind("Id,Summary,CustomerId")] SupportIncident supportIncident)
  {
    var url = $"{Request.Scheme}://{Request.Host}";
    if (supportIncident.CustomerId != 0) {
      supportIncident.Customer = await _context.Customer.FindAsync(supportIncident.CustomerId);
    }
    if (ModelState.IsValid) {
      using var dbContextTransaction = _context.Database.BeginTransaction();
      try {
        string returnUrl = nameof(Index);
        if (supportIncident.Customer!.HasActiveSupportPlan) {
          supportIncident.State = State.InProgress;
          returnUrl = nameof(ListInProgress);
        }
        else {
          supportIncident.State = State.Pending;

          // TODO: generate hash and save to db.
          await _email.SendEmailAsync(
            supportIncident.Customer!.Email,
            "Pending Support Launch",
            supportIncident.Summary,
            supportIncident.State,
            supportIncident.Customer.Name);
        }

        _context.Add(supportIncident);
        await _context.SaveChangesAsync();

        await dbContextTransaction.CommitAsync();

        return RedirectToAction(returnUrl);
      }
      catch (Exception ex) {
        await dbContextTransaction.RollbackAsync();
        // TODO: Missing an exception middleware.
        throw new Exception(ex.Message, ex);
      }
      
    }

    CreateViewBagForDevOrProd();

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
  public async Task<IActionResult> Email(int id, [Bind("Id,Summary,CustomerId,State")] SupportIncident supportIncident)
  {
    if (id != supportIncident.Id) {
      return NotFound();
    }

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

  public async Task<IActionResult> Approval(int? id, string stateSource = "pending")
  {
    ViewBag.StateSource = stateSource;
    ViewBag.pageReturn = stateSource == "awaiting" ? nameof(AwaitingList) : nameof(Index);
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
  public async Task<IActionResult> Approval(int id, [Bind("Id,Summary,CustomerId")] SupportIncident supportIncident, string state, string stateSource)
  {
    if (id != supportIncident.Id) {
      return NotFound();
    }

    supportIncident.Customer = await _context.Customer.FindAsync(supportIncident.CustomerId);

    if (supportIncident.Customer == null) {
      return NotFound("The customer does not exist");
    }

    bool hasActiveSupportPlan = supportIncident.Customer.HasActiveSupportPlan;
    bool isFinanciaApproval = stateSource == "awaiting";

    supportIncident.State = state switch
    {
      "approval" when isFinanciaApproval => State.InProgress,
      "approval" when hasActiveSupportPlan => State.InProgress,
      "approval" => State.Awaiting,
      "rejected" => State.Rejected,
      _ => State.Pending,
    };

    if (ModelState.IsValid) {
      using var dbContextTransaction = _context.Database.BeginTransaction();
      try {
        _context.Update(supportIncident);
        await _context.SaveChangesAsync();

        // TODO: generate hash and save to db.
        // UNDONE: Change is Financial Approval
        var nameState = isFinanciaApproval ? State.OnHold.GetDisplayName() : supportIncident.State.GetDisplayName();
        // Send finance email only if you do not have active support
        // UNDONE: Change is Financial Approval
        if (supportIncident.State == State.Awaiting || isFinanciaApproval) {
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

      string nameAction = isFinanciaApproval ? nameof(AwaitingList) : nameof(Index);
      return RedirectToAction(nameAction);
    }

    ViewBag.pageReturn = stateSource == "awaiting" ? nameof(AwaitingList) : nameof(Index);

    return View(supportIncident);
  }

  // GET: SupportIncident/AssociateProject
  public async Task<IActionResult> AssociateProject(int? id, string sourceView)
  {
    ViewBag.SourceView = sourceView;
    CreateViewBagForDevOrProd();

    if (id == null || _context.Project == null) {
      return NotFound();
    }

    var supportIncident = await _context.SupportIncident
      .Include(c => c.Customer)
      .Include(p => p.Project)
      .FirstOrDefaultAsync(s => s.Id == id);
        
    if (supportIncident == null) {
      return NotFound();
    }

    return View(supportIncident);
  }

  // POST: SupportIncident/AssociateProject
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> AssociateProject([Bind("Id,Summary,CustomerId,State,ProjectId")] SupportIncident supportIncident)
  {
    if (supportIncident.CustomerId != 0) {
      supportIncident.Customer = await _context.Customer.FindAsync(supportIncident.CustomerId);
    }
    if (supportIncident.ProjectId != 0) {
      supportIncident.Project = await _context.Project.FindAsync(supportIncident.ProjectId);
    }
    if (ModelState.IsValid) {
      using var dbContextTransaction = _context.Database.BeginTransaction();
      try {
        _context.Update(supportIncident);
        await _context.SaveChangesAsync();

        await dbContextTransaction.CommitAsync();

        return RedirectToAction(nameof(ListInProgress));
      }
      catch (Exception ex) {
        await dbContextTransaction.RollbackAsync();
        // TODO: Missing an exception middleware.
        throw new Exception(ex.Message, ex);
      }

    }

    CreateViewBagForDevOrProd();

    return View(supportIncident);
  }

  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Solved,SolvedClose")]
  public async Task<IActionResult> Close(int? id)
  {
    if (id == null || _context.SupportIncident == null) {
      return NotFound();
    }

    var supportincident = await _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Interventions)
      .Include(s => s.Project)
      .FirstOrDefaultAsync(m => m.Id == id);
    if (supportincident == null) {
      return NotFound();
    }

    return View(supportincident);
  }

  // POST: SupportIncident/Close
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Solved,SolvedClose")]
  public async Task<IActionResult> Close([Bind("Id,Summary,CustomerId,State,Total,ProjectId")] SupportIncident supportIncident)
  {
    if (supportIncident.CustomerId != 0) {
      supportIncident.Customer = await _context.Customer.FindAsync(supportIncident.CustomerId);
    }
    if (supportIncident.ProjectId != 0) {
      supportIncident.Project = await _context.Project.FindAsync(supportIncident.ProjectId);
    }
    if (ModelState.IsValid) {
      using var dbContextTransaction = _context.Database.BeginTransaction();
      try {
        supportIncident.State = State.Solved;
        _context.Update(supportIncident);
        await _context.SaveChangesAsync();

        await dbContextTransaction.CommitAsync();

        return RedirectToAction(nameof(SolvedList));
      }
      catch (Exception ex) {
        await dbContextTransaction.RollbackAsync();
        // TODO: Missing an exception middleware.
        throw new Exception(ex.Message, ex);
      }

    }

    return View(supportIncident);
  }

  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Solved,SolvedReOpen")]
  public async Task<IActionResult> ReOpen(int? id)
  {
    if (id == null || _context.SupportIncident == null) {
      return NotFound();
    }

    var supportincident = await _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Interventions)
      .Include(s => s.Project)
      .FirstOrDefaultAsync(m => m.Id == id);
    if (supportincident == null) {
      return NotFound();
    }

    return View(supportincident);
  }

  // POST: SupportIncident/Close
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Solved,SolvedReOpen")]
  public async Task<IActionResult> ReOpen([Bind("Id,Summary,CustomerId,State,Total,ProjectId")] SupportIncident supportIncident)
  {
    if (supportIncident.CustomerId != 0) {
      supportIncident.Customer = await _context.Customer.FindAsync(supportIncident.CustomerId);
    }
    if (supportIncident.ProjectId != 0) {
      supportIncident.Project = await _context.Project.FindAsync(supportIncident.ProjectId);
    }
    if (ModelState.IsValid) {
      using var dbContextTransaction = _context.Database.BeginTransaction();
      try {
        supportIncident.State = State.InProgress;
        _context.Update(supportIncident);
        await _context.SaveChangesAsync();

        await dbContextTransaction.CommitAsync();

        return RedirectToAction(nameof(ListInProgress));
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
  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Solved,SolvedDelete")]
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
  [Authorize(Roles = "SuperAdmin,Admin,SupportIncident,SupportIncidentAdd,SupportIncidentEdit,SupportIncidentDelete," +
    "Solved,SolvedDelete")]
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

  private void CreateViewBagForDevOrProd()
  {
    if (_env.IsDevelopment()) {
      ViewBag.UrlClient = "";
    }
    else {
      ViewBag.UrlClient = "/bubblevel";
    }
  }
}