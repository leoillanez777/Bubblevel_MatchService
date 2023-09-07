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

public class SettingController : Controller {
  private readonly ApplicationDbContext _context;

  public SettingController(ApplicationDbContext context)
  {
    _context = context;
  }

  // GET: Setting
  public async Task<IActionResult> Index()
  {
    return _context.Setting != null ?
                View(await _context.Setting.OrderBy(s => s.State).ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.Setting'  is null.");
  }

  // GET: Setting/Details/5
  public async Task<IActionResult> Details(int? id)
  {
    if (id == null || _context.Setting == null) {
      return NotFound();
    }

    var setting = await _context.Setting
        .FirstOrDefaultAsync(m => m.Id == id);
    if (setting == null) {
      return NotFound();
    }

    return View(setting);
  }

  // GET: Setting/Create
  public IActionResult Create()
  {
    PopulateDropdownLists();
    return View();
  }

  // POST: Setting/Create
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create([Bind("Id,State,DeliveryService,EmailSender,Name,NameReceiver,Summary,ResponseByEmail")] Setting setting)
  {
    if (ModelState.IsValid) {

      if (!SettingExists(setting.State)) {
        _context.Add(setting);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      else {
        ModelState.AddModelError("State", "State exists");
      }
    }

    PopulateDropdownLists();
    return View(setting);
  }

  // GET: Setting/Edit/5
  public async Task<IActionResult> Edit(int? id)
  {
    if (id == null || _context.Setting == null) {
      return NotFound();
    }

    PopulateDropdownLists();

    var setting = await _context.Setting.FindAsync(id);
    if (setting == null) {
      return NotFound();
    }
    return View(setting);
  }

  // POST: Setting/Edit/5
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, [Bind("Id,State,DeliveryService,EmailSender,Name,NameReceiver,Summary,ResponseByEmail")] Setting setting)
  {
    if (id != setting.Id) {
      return BadRequest("Url not exists");
    }

    if (!ModelState.IsValid) {
      PopulateDropdownLists();
      return View(setting);
    }

    bool canEdit = CanEditState(setting.State, id);

    if (!canEdit) {
      PopulateDropdownLists();
      ModelState.AddModelError("State", "Cannot edit state");
      return View(setting);
    }

    try {
      await UpdateEmailSetting(setting);
      return RedirectToAction(nameof(Index));
    }
    catch (DbUpdateConcurrencyException) {
      if (!SettingExists(setting.Id)) {
        return NotFound();
      }
      else {
        throw;
      }
    }
      
  }

  // GET: Setting/Delete/5
  public async Task<IActionResult> Delete(int? id)
  {
    if (id == null || _context.Setting == null) {
      return NotFound();
    }

    var setting = await _context.Setting
        .FirstOrDefaultAsync(m => m.Id == id);
    if (setting == null) {
      return NotFound();
    }

    return View(setting);
  }

  // POST: Setting/Delete/5
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    if (_context.Setting == null) {
      return Problem("Entity set 'ApplicationDbContext.Setting'  is null.");
    }
    var setting = await _context.Setting.FindAsync(id);
    if (setting != null) {
      _context.Setting.Remove(setting);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
  }

  private bool SettingExists(int id)
  {
    return (_context.Setting?.Any(e => e.Id == id)).GetValueOrDefault();
  }

  private bool SettingExists(State state)
  {
    return (_context.Setting?.Any(e => e.State == state)).GetValueOrDefault();
  }

  private bool CanEditState(State state, int settingId)
  {
    return (_context.Setting?.Any(e => e.State == state && e.Id == settingId)).GetValueOrDefault();
  }

  private void PopulateDropdownLists()
  {
    ViewBag.State = new SelectList(Enum.GetValues(typeof(State)));
    ViewBag.Emailing = new SelectList(Enum.GetValues(typeof(Emailing)));
  }

  private async Task UpdateEmailSetting(Setting setting)
  {
    _context.Update(setting);
    await _context.SaveChangesAsync();
  }
}
