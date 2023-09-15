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

public class CustomerController : Controller {
  private readonly ApplicationDbContext _context;

  public CustomerController(ApplicationDbContext context)
  {
    _context = context;
  }

  // GET: Customer
  [Authorize(Roles = "SuperAdmin,Admin,Customer,CustomerAdd,CustomerEdit,CustomerDelete")]
  public async Task<IActionResult> Index()
  {
    return _context.Customer != null ?
                View(await _context.Customer.ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.Customer'  is null.");
  }

  // GET: Customer/GetCustomer
  // Json data
  [AllowAnonymous]
  public async Task<IActionResult> GetCustomer(string? filter)
  {
    filter = filter?.ToLower();

    IQueryable<Customer> query = _context.Customer;

    if (!string.IsNullOrEmpty(filter)) {
      query = query.Where(c => c.Name.ToLower().Contains(filter) || c.Email.ToLower().Contains(filter));
    }
    else {
      query = query.Take(10);
    }

    var filteredCustomers = await query
        .Select(c => new { c.Id, NameWithEmail = $"{c.Name} ({c.Email})" })
        .ToListAsync();

    return Json(filteredCustomers);
  }

  // GET: Customer/Details/5
  [Authorize(Roles = "SuperAdmin,Admin,Customer,CustomerAdd,CustomerEdit,CustomerDelete")]
  public async Task<IActionResult> Details(int? id)
  {
    if (id == null || _context.Customer == null) {
      return NotFound();
    }

    var customer = await _context.Customer
        .FirstOrDefaultAsync(m => m.Id == id);
    if (customer == null) {
      return NotFound();
    }

    return View(customer);
  }

  // GET: Customer/Create
  [Authorize(Roles = "SuperAdmin,Admin,Customer,CustomerAdd")]
  public IActionResult Create()
  {
    return View();
  }

  // POST: Customer/Create
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(Roles = "SuperAdmin,Admin,Customer,CustomerAdd")]
  public async Task<IActionResult> Create([Bind("Id,Name,Email,HasActiveSupportPlan")] Customer customer)
  {
    if (ModelState.IsValid) {
      if (CustomerEmailExists(customer.Email)) {
        ModelState.AddModelError("Email", "The email already exists in the database.");
        return View(customer);
      }
      _context.Add(customer);
      await _context.SaveChangesAsync();
      
      return RedirectToAction(nameof(Index));
    }
    return View(customer);
  }

  [HttpPost]
  [AllowAnonymous]
  public async Task<IActionResult> CreateCustomer([FromBody]CustomerDTO customerDTO)
  {

    if (customerDTO == null) {
      return BadRequest("Invalid data");
    }

    string msg = "OK", idCustomer = "";
    if (CustomerEmailExists(customerDTO.Email)) {
      msg = "The email already exists in the database.";
    }
    else {
      Customer customer = new() {
        Name = customerDTO.Name,
        Email = customerDTO.Email,
        HasActiveSupportPlan = false,
        SupportIncidents = null!
      };

      _context.Add(customer);
      await _context.SaveChangesAsync();

      idCustomer = customer.Id.ToString();
    }
    var dataObject = new { Id = idCustomer, NameWithEmail = $"{customerDTO.Name} ({customerDTO.Email})" };
    return Json(new { msg, dataObject });
  }

  // GET: Customer/Edit/5
  [Authorize(Roles = "SuperAdmin,Admin,Customer,CustomerEdit")]
  public async Task<IActionResult> Edit(int? id)
  {
    if (id == null || _context.Customer == null) {
      return NotFound();
    }

    var customer = await _context.Customer.FindAsync(id);
    if (customer == null) {
      return NotFound();
    }
    return View(customer);
  }

  // POST: Customer/Edit/5
  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(Roles = "SuperAdmin,Admin,Customer,CustomerEdit")]
  public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,HasActiveSupportPlan")] Customer customer)
  {
    if (id != customer.Id) {
      return NotFound();
    }

    if (ModelState.IsValid) {
      try {

        if (CustomerEmailExists(customer.Email, customer.Id)) {
          ModelState.AddModelError("Email", "The email already exists in the database.");
          return View(customer);
        }

        _context.Update(customer);
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) {
        if (!CustomerExists(customer.Id)) {
          return NotFound();
        }
        else {
          throw;
        }
      }
      return RedirectToAction(nameof(Index));
    }
    return View(customer);
  }

  // GET: Customer/Delete/5
  [Authorize(Roles = "SuperAdmin,Admin,Customer,CustomerDelete")]
  public async Task<IActionResult> Delete(int? id)
  {
    if (id == null || _context.Customer == null) {
      return NotFound();
    }

    var customer = await _context.Customer
        .FirstOrDefaultAsync(m => m.Id == id);
    if (customer == null) {
      return NotFound();
    }

    return View(customer);
  }

  // POST: Customer/Delete/5
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  [Authorize(Roles = "SuperAdmin,Admin,Customer,CustomerDelete")]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    if (_context.Customer == null) {
      return Problem("Entity set 'ApplicationDbContext.Customer'  is null.");
    }
    var customer = await _context.Customer.FindAsync(id);
    if (customer != null) {
      _context.Customer.Remove(customer);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
  }

  private bool CustomerExists(int id)
  {
    return (_context.Customer?.Any(e => e.Id == id)).GetValueOrDefault();
  }

  private bool CustomerEmailExists(string email, int? customerId = null)
  {
    return _context.Customer
      .Where(c => c.Email.ToLower() == email.ToLower())
      .Any(c => customerId == null || c.Id != customerId);
  }
}
