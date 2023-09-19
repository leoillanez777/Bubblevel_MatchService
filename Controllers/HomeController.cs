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
using System.Text.RegularExpressions;

namespace Bubblevel_MatchService.Controllers;

[Authorize]
public partial class HomeController : Controller {

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
    ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
    ViewBag.StateSearch = string.IsNullOrEmpty(stateSearch) ? null : stateSearch;
    ViewBag.CustomerSearch = string.IsNullOrEmpty(customerSearch) ? null : customerSearch;
    ViewBag.ProjectSearch = string.IsNullOrEmpty(projectSearch) ? null : projectSearch;
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
    csv.AppendLine(BuildHeader());

    foreach (var row in incidents) {

      IncidentCsvRecord csvRecord = new(row.Id.ToString()) {
        State = row.State.GetDisplayName(),
        Summary = MyRegex().Replace(row.Summary, " "),
        Customer = row.Customer?.Name ?? string.Empty,
        Project = row.Project?.Name ?? string.Empty,
        ProjectDuration = (row.Project?.Duration ?? decimal.Zero).ToString(),
      };
      //Add comments.
      var commentsData = row.Comments?.Select(c => MyRegex().Replace(c.Text, " ")).ToList();
      csvRecord.Comments = string.Join(";", commentsData ?? new List<string>());

      var interventionsData = row.Interventions?.Select(i => new InterventionList
      {
        Date = i.InterventionDate.ToString(),
        Description = MyRegex().Replace(i.Description, " "),
        Duration = i.Duration.ToString()
      });

      if (interventionsData != null && interventionsData.Any()) {
        bool firstIntervention = true;
        foreach (var i in interventionsData!) {
          if (firstIntervention) {
            firstIntervention = false;
            csvRecord.ConCatIntervention(i);
            csv.AppendLine(csvRecord.Format());
          }
          else {
            IncidentCsvRecord csvIntervention = new(row.Id.ToString());
            csvIntervention.ConCatIntervention(i);
            csv.AppendLine(csvIntervention.Format());
          }
        }
      }
      else {
        csv.AppendLine(csvRecord.Format());
      }
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

  public class InterventionList
  {
    public string Date { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Duration { get; set; } = null!;
  }

  public class CsvColumns {
    public const string IncidentNumber = "Incident Number";
    public const string State = "State";
    public const string Summary = "Summary";
    public const string Customer = "Customer";
    public const string Project = "Project";
    public const string ProjectDuration = "Project Duration";
    public const string InterventionDate = "Intervention Date";
    public const string InterventionDescription = "Intervention Description";
    public const string InterventionDuration = "Intervention Duration";
    public const string Comments = "Comments";
  }

  public class IncidentCsvRecord {
    public string IncidentNumber { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string ProjectDuration { get; set; } = string.Empty;
    public string InterventionDate { get; set; } = string.Empty;
    public string InterventionDescription { get; set; } = string.Empty;
    public string InterventionDuration { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;

    public IncidentCsvRecord(string id)
    {
      IncidentNumber = id;
    }

    public void ConCatIntervention(InterventionList list)
    {
      InterventionDate = list.Date;
      InterventionDescription = list.Description;
      InterventionDuration = list.Duration;
    }

    public string Format()
    {
      return $"{IncidentNumber};{State};{Summary};{Customer};{Project};{ProjectDuration};" +
        $"{InterventionDate};{InterventionDescription};{InterventionDuration};{Comments}";
    }
  }

  private static string BuildHeader()
  {
    return $"{CsvColumns.IncidentNumber};{CsvColumns.State};{CsvColumns.Summary};" +
      $"{CsvColumns.Customer};{CsvColumns.Project};{CsvColumns.ProjectDuration};{CsvColumns.InterventionDate};" +
      $"{CsvColumns.InterventionDescription};{CsvColumns.InterventionDuration};{CsvColumns.Comments}";
  }

  [GeneratedRegex("[\\n\\t\\r]")]
  private static partial Regex MyRegex();
}

