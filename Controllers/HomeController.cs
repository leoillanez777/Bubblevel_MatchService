using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bubblevel_MatchService.Models;
using Microsoft.AspNetCore.Authorization;
using Bubblevel_MatchService.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Globalization;
using Microsoft.CodeAnalysis;
using System.Text.Json;

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
    LatestStatistics();
    AvgSupport();
    SupportByState();
    DurationVsProject();
    return View();
  }

  public IActionResult Privacy()
  {
    return View();
  }

  [Authorize(Roles = "SuperAdmin,Admin,Report")]
  public async Task<IActionResult> Report(string sortOrder, string stateSearch, string customerSearch, string projectSearch, int? page)
  {
    if (!string.IsNullOrEmpty(stateSearch) || !string.IsNullOrEmpty(customerSearch)) {
      page = 1;
    }

    ViewBag.CurrentSort = sortOrder;
    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
    ViewBag.StateSearch = String.IsNullOrEmpty(stateSearch) ? null : stateSearch;
    ViewBag.CustomerSearch = String.IsNullOrEmpty(customerSearch) ? null : customerSearch;
    ViewBag.ProjectSearch = String.IsNullOrEmpty(projectSearch) ? null : projectSearch;
    CreateViewBagForDevOrProd();

    var query = _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Interventions)
      .Include(s => s.Project)
      .AsQueryable();

    if (!string.IsNullOrEmpty(stateSearch)) {
      var stateIds = stateSearch.Split(",").Select(Enum.Parse<State>).ToList();
      if (stateIds.Any()) {
        query = query.Where(s => stateIds.Contains(s.State));
      }
    }

    if (!string.IsNullOrEmpty(customerSearch)) {
      var customerIds = customerSearch.Split(",").Select(int.Parse).ToList();
      if (customerIds.Any()) {
        query = query.Where(s => customerIds.Contains(s.CustomerId));
      }
    }

    if (!string.IsNullOrEmpty(projectSearch)) {
      var projectIds = projectSearch.Split(",").Select(int.Parse).ToList();
      if (projectIds.Any()) {
        query = query.Where(s => s.ProjectId != null && projectIds.Contains(s.ProjectId.Value));
      }
    }

    int pageSize = 10;
    int pageNumber = page ?? 1;
    return View(await query.ToPagedListAsync(pageNumber, pageSize));
  }

  [HttpPost]
  public async Task<IActionResult> ReportExport(string stateSearch, string customerSearch, string projectSearch)
  {
    
    var query = _context.SupportIncident
      .Include(s => s.Customer)
      .Include(s => s.Interventions)
      .Include(s => s.Project)
      .Include(s => s.Comments)
      .AsQueryable();

    if (!string.IsNullOrEmpty(stateSearch)) {
      var stateIds = stateSearch.Split(",").Select(Enum.Parse<State>).ToList();
      if (stateIds.Any()) {
        query = query.Where(s => stateIds.Contains(s.State));
      }
    }

    if (!string.IsNullOrEmpty(customerSearch)) {
      var customerIds = customerSearch.Split(",").Select(int.Parse).ToList();
      if (customerIds.Any()) {
        query = query.Where(s => customerIds.Contains(s.CustomerId));
      }
    }

    if (!string.IsNullOrEmpty(projectSearch)) {
      var projectIds = projectSearch.Split(",").Select(int.Parse).ToList();
      if (projectIds.Any()) {
        query = query.Where(s => s.ProjectId != null && projectIds.Contains(s.ProjectId.Value));
      }
    }

    var incidents = await query.ToListAsync();

    // Create CSV 
    var csv = new System.Text.StringBuilder();
    csv.AppendLine("State;Incident Number;Summary;Customer;Project;Duration Project;Interventions;Comments"); // Header

    foreach (var row in incidents) {
      var commonData = $"{row.State.GetDisplayName()};{row.Id};{row.Summary};" +
        $"{(row.Customer is not null ? row.Customer.Name: string.Empty)};" +
        $"{(row.Project is not null ? row.Project.Name : string.Empty)};" +
        $"{(row.Project is not null ? row.Project.Duration : decimal.Zero)}";

      var interventionsData = row.Interventions?.Select(i => new
      {
        date = i.InterventionDate,
        description = i.Description,
        duration = i.Duration
      });

      var commentsData = row.Comments?.Select(c => new
      {
        summary = c.Text
      });

      var interventionsJson = JsonSerializer.Serialize(interventionsData);
      var commentsJson = JsonSerializer.Serialize(commentsData);

      csv.AppendLine($"{commonData};{interventionsJson};{commentsJson}");
    }

    // Setear headers para que se descargue como CSV
    return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "data.csv");
  }

  // GET: Home/GetState
  // Json data
  [AllowAnonymous]
  public IActionResult GetState()
  {
    IQueryable<State> query = Enum.GetValues<State>().AsQueryable();

    var filteredCustomers = query
        .Select(s => new {
          Id = s,
          Name = s.GetDisplayName()
        }).ToList();

    return Json(filteredCustomers);
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

  #region DashBoard

  private void LatestStatistics()
  {
    int currentYear = DateTime.Now.Year;
    try {
      var statistics = _context.Intervention
      .GroupBy(i => i.InterventionDate.Month)
      .Select(g => new MonthlyStatistics {
        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
        Count = g.Count()
      })
      .ToList();

      ViewBag.Statistics = statistics;
    }
    catch {
      ViewBag.Statistics = new MonthlyStatistics();
    }
  }

  private void AvgSupport()
  {
    try {
      var avgSupport = _context.SupportIncident
      .GroupBy(s => s.CustomerId)
      .Select(g => new MonthlyStatistics {
        MonthName = g.FirstOrDefault()!.Customer!.Name,
        Count = g.Count()
      })
      .ToList();
      ViewBag.AvgSupport = avgSupport;
    }
    catch {
      ViewBag.AvgSupport = new MonthlyStatistics();
    }
  }

  private void SupportByState()
  {
    try {
      var supportState = _context.SupportIncident
        .GroupBy(s => s.State)
        .Select(g => new MonthlyStatistics {
          MonthName = g.FirstOrDefault()!.State.GetDisplayName(),
          Count = g.Count()
        })
        .ToList();
      ViewBag.SupportState = supportState;
    }
    catch {
      ViewBag.SupportState = new MonthlyStatistics();
    }

  }

  private void DurationVsProject()
  {
    try {
      var projectStats = _context.Project
        .Select(p => new ProjectStatistics {
          ProjectId = p.Id,
          ProjectName = p.Name,
          ProjectDuration = p.Duration
        })
        .ToList();

      var interventionDurations = _context.Intervention
        .GroupBy(i => i.SupportIncident!.ProjectId)
        .Select(g => new {
          ProjectId = g.Key,
          TotalDuration = g.Sum(i => i.Duration)
        });

      foreach (var stats in projectStats) {
        stats.TotalInterventionsDuration = interventionDurations
           .FirstOrDefault(d => d.ProjectId == stats.ProjectId)?.TotalDuration ?? decimal.Zero;
      }
      ViewBag.ProjectStats = projectStats;
    }
    catch {
      ViewBag.ProjectStats = new ProjectStatistics();
    }

  }


  public class MonthlyStatistics {
    public string MonthName { get; set; } = null!;
    public int Count { get; set; }
  }

  public class ProjectStatistics {
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public decimal ProjectDuration { get; set; }
    public decimal TotalInterventionsDuration { get; set; }
  }

  #endregion

  public class StateList
  {
    public string Value { get; set; } = null!;
    public string Text { get; set; } = null!;
  }
}

