using PersonalFinance.Api.Models.Goals;
using PersonalFinance.Api.Models.Transactions;

using Microsoft.EntityFrameworkCore;

namespace PersonalFinance.Api.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Goal> Goals { get; set; }
}   

