using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bubblevel_MatchService.Models;
using Microsoft.AspNetCore.Authorization;
using Bubblevel_MatchService.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Bubblevel_MatchService.Controllers;

[Authorize]
public class HomeController : Controller {

  private readonly ILogger<HomeController> _logger;
  private readonly ApplicationDbContext _context;
  private readonly IWebHostEnvironment _env;

  public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment env)
  {
    _logger = logger;
    _context = context;
    _env = env;
  }

  public IActionResult Index()
  {
    return View();
  }

  public IActionResult Privacy()
  {
    return View();
  }

  [Authorize(Roles = "SuperAdmin,Admin,Report")]
  public async Task<IActionResult> Report(string sortOrder, string stateSearch, string customerSearch, string projectSearch, int? page)
  {
    if (stateSearch != null || customerSearch != null) {
      page = 1;
    }

    ViewBag.CurrentSort = sortOrder;
    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
    ViewBag.StateSearch = String.IsNullOrEmpty(stateSearch) ? null : stateSearch;
    ViewBag.CustomerSearch = String.IsNullOrEmpty(customerSearch) ? null : customerSearch;
    ViewBag.ProjectSearch = String.IsNullOrEmpty(projectSearch) ? null : projectSearch;

    List<StateList> stateLists = new() { new StateList() { Value = "", Text = "ALL"} };
    foreach (var item in Enum.GetValues<State>()) {
      stateLists.Add(new StateList() {
        Text = item.GetDisplayName(),
        Value = item.ToString()
      });
    }
    ViewData["States"] = new SelectList(stateLists, "Value", "Text", stateSearch);
    CreateViewBagForDevOrProd();

    var applicationDbContext = await _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Interventions)
      .Include(s => s.Project)
      .ToListAsync();

    if (stateSearch != null) {
      State stateFind = Enum.Parse<State>(stateSearch);
      applicationDbContext = applicationDbContext.Where(s => s.State == stateFind).ToList();
    }

    if (customerSearch != null) {
      string[] listId = customerSearch.Split(",");
      List<int> customerIds = listId.Select(int.Parse).ToList();

      if (customerIds.Any()) {
        applicationDbContext = applicationDbContext
          .Where(s => customerIds.Contains(s.CustomerId))
          .ToList();
      }
    }

    if (projectSearch != null) {
      string[] listProjectId = projectSearch.Split(",");
      List<int> projectIds = listProjectId.Select(int.Parse).ToList();

      if (projectIds.Any()) {
        applicationDbContext = applicationDbContext
          .Where(s => s.ProjectId != null && projectIds.Contains(s.ProjectId.Value))
          .ToList();
      }
    }

    int pageSize = 10;
    int pageNumber = page ?? 1;
    return View(await applicationDbContext.ToPagedListAsync(pageNumber, pageSize));
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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


  public class StateList
  {
    public string Value { get; set; } = null!;
    public string Text { get; set; } = null!;
  }
}

