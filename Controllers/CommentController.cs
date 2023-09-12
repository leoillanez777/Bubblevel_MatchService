using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bubblevel_MatchService.Context;
using Bubblevel_MatchService.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Bubblevel_MatchService.Controllers;

public class CommentController : Controller {
  private readonly ApplicationDbContext _context;

  public CommentController(ApplicationDbContext context)
  {
    _context = context;
  }

  // GET: Comment
  public async Task<IActionResult> Index(int supportId, string name, int? page)
  {
    //// get source view and save in claims.
    //var userManager = HttpContext.RequestServices
    //                            .GetRequiredService<UserManager<Areas.Identity.Data.ApplicationUser>>();
    //// Obtenemos el usuario actual
    //var user = await userManager.GetUserAsync(HttpContext.User);
    //Claim sourceViewClaim = new("SourceView", "John");
    //await userManager.AddClaimAsync(user, sourceViewClaim);

    ViewData["SupportIncidentId"] = supportId;
    ViewData["CustomerName"] = name;
    int pageSize = 10;
    int pageNumber = page ?? 1;
    var applicationDbContext = _context.Comment
      .Where(c => c.SupportIncidentId == supportId)
      .OrderByDescending(c => c.Id);
    return View(await applicationDbContext.ToPagedListAsync(pageNumber, pageSize));
  }

  // GET: Comment/Details/5
  public async Task<IActionResult> Details(int? id, int supportId, string name)
  {

    ViewData["SupportIncidentId"] = supportId;
    ViewData["CustomerName"] = name;

    if (id == null || _context.Comment == null) {
      return NotFound();
    }

    var comment = await _context.Comment
        .Include(c => c.SupportIncident)
        .FirstOrDefaultAsync(m => m.Id == id);
    if (comment == null) {
      return NotFound();
    }

    return View(comment);
  }

  // GET: Comment/Create
  public IActionResult Create(int supportId, string name)
  {
    ViewData["SupportIncidentId"] = supportId;
    ViewData["CustomerName"] = name;
    return View();
  }

  // POST: Comment/Create
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create([Bind("Id,Text,CreatedAt,SupportIncidentId")] Comment comment, string name)
  {
    ViewData["SupportIncidentId"] = comment.SupportIncidentId;
    ViewData["CustomerName"] = name;
    if (ModelState.IsValid) {
      comment.CreatedAt = DateTime.Now;
      comment.CreatedBy = User.Identity!.Name ?? "";
      _context.Add(comment);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index), new { supportId = comment.SupportIncidentId, name });
    }
    return View(comment);
  }

  // GET: Comment/Edit/5
  public async Task<IActionResult> Edit(int supportId, string name, int? id)
  {
    ViewData["SupportIncidentId"] = supportId;
    ViewData["CustomerName"] = name;

    if (id == null || _context.Comment == null) {
      return NotFound();
    }

    var comment = await _context.Comment.FindAsync(id);
    if (comment == null) {
      return NotFound();
    }
    return View(comment);
  }

  // POST: Comment/Edit/5
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, [Bind("Id,Text,CreatedAt,SupportIncidentId")] Comment comment, string name)
  {

    ViewData["SupportIncidentId"] = comment.SupportIncidentId;
    ViewData["CustomerName"] = name;

    if (id != comment.Id) {
      return NotFound();
    }

    if (ModelState.IsValid) {
      try {
        comment.CreatedAt = DateTime.Now;
        comment.CreatedBy = User.Identity!.Name ?? "";
        _context.Update(comment);
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) {
        if (!CommentExists(comment.Id)) {
          return NotFound();
        }
        else {
          throw;
        }
      }
      return RedirectToAction(nameof(Index), new { supportId = comment.SupportIncidentId, name });
    }
    return View(comment);
  }

  // GET: Comment/Delete/5
  public async Task<IActionResult> Delete(int? id, int supportId, string name)
  {

    ViewData["SupportIncidentId"] = supportId;
    ViewData["CustomerName"] = name;

    if (id == null || _context.Comment == null) {
      return NotFound();
    }

    var comment = await _context.Comment
        .Include(c => c.SupportIncident)
        .FirstOrDefaultAsync(m => m.Id == id);
    if (comment == null) {
      return NotFound();
    }

    return View(comment);
  }

  // POST: Comment/Delete/5
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id, string supportId, string name)
  {
    ViewData["SupportIncidentId"] = supportId;
    ViewData["CustomerName"] = name;

    if (_context.Comment == null) {
      return Problem("Entity set 'ApplicationDbContext.Comment'  is null.");
    }
    var comment = await _context.Comment.FindAsync(id);
    if (comment != null) {
      _context.Comment.Remove(comment);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index), new { supportId = supportId, name });
  }

  private bool CommentExists(int id)
  {
    return (_context.Comment?.Any(e => e.Id == id)).GetValueOrDefault();
  }
}
