using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bubblevel_MatchService.Context;
using Bubblevel_MatchService.Models;

namespace Bubblevel_MatchService.Controllers;

public class EmailSettingController : Controller {
  private readonly ApplicationDbContext _context;

  public EmailSettingController(ApplicationDbContext context)
  {
    _context = context;
  }

  // GET: EmailSetting/Create
  public async Task<IActionResult> Create()
  {
    var emailSetting = await _context.EmailSetting.FirstOrDefaultAsync(m => true);
    if (emailSetting != null) {
      return RedirectToAction(nameof(Edit), new { emailSetting.Id });
    }
      
    return View();
  }

  // POST: EmailSetting/Create
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create([Bind("Id,Host,Port,Username,Password,UseSsl,UseTls")] EmailSetting emailSetting)
  {
    if (ModelState.IsValid) {
      _context.Add(emailSetting);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index), "Home");
    }
    return View(emailSetting);
  }

  // GET: EmailSetting/Edit/5
  public async Task<IActionResult> Edit(int? id)
  {
    if (id == null || _context.EmailSetting == null) {
      return NotFound();
    }

    var emailSetting = await _context.EmailSetting.FindAsync(id);
    if (emailSetting == null) {
      return NotFound();
    }
    return View(emailSetting);
  }

  // POST: EmailSetting/Edit/5
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, [Bind("Id,Host,Port,Username,Password,UseSsl,UseTls")] EmailSetting emailSetting)
  {
    if (id != emailSetting.Id) {
      return NotFound();
    }

    if (ModelState.IsValid) {
      try {
        _context.Update(emailSetting);
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) {
        if (!EmailSettingExists(emailSetting.Id)) {
          return NotFound();
        }
        else {
          throw;
        }
      }
      return RedirectToAction(nameof(Index), "Home");
    }
    return View(emailSetting);
  }

  

  private bool EmailSettingExists(int id)
  {
    return (_context.EmailSetting?.Any(e => e.Id == id)).GetValueOrDefault();
  }
}
