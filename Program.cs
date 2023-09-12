global using Bubblevel_MatchService.Areas.Identity.Data;
using Bubblevel_MatchService.Context;
using Bubblevel_MatchService.Services;
using Bubblevel_MatchService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add configuration to Sql Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("SqlLocal"))
);

#region Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
  .AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options => {
  // Password settings
  options.Password.RequireDigit = true;
  options.Password.RequireLowercase = true;
  options.Password.RequireNonAlphanumeric = true;
  options.Password.RequireUppercase = true;
  options.Password.RequiredLength = 6;
  options.Password.RequiredUniqueChars = 1;

  // Lockout settings
  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
  options.Lockout.MaxFailedAccessAttempts = 5;
  options.Lockout.AllowedForNewUsers = true;

  // User settings
  options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
  options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options => {
  // Cookie settings
  options.Cookie.HttpOnly = true;
  options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
  options.LoginPath = "/Identity/Account/Login";
  options.AccessDeniedPath = "/Identity/Account/AccessDenied";
  options.SlidingExpiration = true;
});
#endregion

// Add services to the container.
builder.Services.AddTransient<IEmailSettingsRepository, EmailSettingsRepositoryService>();
builder.Services.AddTransient<IEmailSender, EmailSenderService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}
else {
  using var scope = app.Services.CreateScope();
  var services = scope.ServiceProvider;
  await SeedData.InitializeAsync(services);
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
  name: "WithSourceView",
  pattern: "{controller=Home}/{action=Index}/{sourceView=Index}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

