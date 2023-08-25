using System;
using Bubblevel_MatchService.Models;
using Microsoft.EntityFrameworkCore;

namespace Bubblevel_MatchService.Context
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
    {
    }

    public DbSet<Customer> Customer { get; set; }
  }
}

