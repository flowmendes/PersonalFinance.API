using PersonalFinance.Api.Models.Transactions;
using PersonalFinance.Api.Models.Goals;
using PersonalFinance.Api.Models.Users;

using Microsoft.EntityFrameworkCore;

namespace PersonalFinance.Api.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Goal> Goals { get; set; }

    public DbSet<User> Users { get; set; }
}   

