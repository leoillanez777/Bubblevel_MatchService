using System;
using Bubblevel_MatchService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bubblevel_MatchService.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.SeedData();
  }

  public DbSet<Comment> Comment { get; set; } = default!;
  public DbSet<Customer> Customer { get; set; } = default!;
  public DbSet<EmailSetting> EmailSetting { get; set; } = default!;
  public DbSet<Project> Project { get; set; } = default!;
  public DbSet<Setting> Setting { get; set; } = default!;
  public DbSet<SupportIncident> SupportIncident { get; set; } = default!;
  public DbSet<Bubblevel_MatchService.Models.Intervention> Intervention { get; set; } = default!;
}
