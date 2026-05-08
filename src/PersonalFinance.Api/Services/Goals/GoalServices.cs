using PersonalFinance.Api.Models.Goals;
using PersonalFinance.Api.DTOs.Goals;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Api.Data;
using PersonalFinance.Api.Models.Transactions;
using System.Data;
using BCrypt.Net;
using PersonalFinance.Api.Models.Users;
using System.Security.Claims;


namespace PersonalFinance.Api.Services.Goals;

public class GoalServices : IGoalServices
{
    private readonly AppDbContext _context;
    private readonly string? _userId;

    public GoalServices(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;

        _userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }


    /// <summary>
    /// Adiciona meta ao banco de dados
    /// </summary>

    public async Task<Goal> AddGoal(CreateGoalDto dto)
    {
        if (string.IsNullOrEmpty(_userId))
        {
            throw new UnauthorizedAccessException("Unidentified user");
        }

        var createGoal = new Goal
        {
            UserId = _userId,
            Title = dto.Title,
            TargetAmount = dto.TargetAmount,
            Deadline = dto.DeadLine,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };

        _context.Goals.Add(createGoal);
        await _context.SaveChangesAsync();

        return createGoal;
    }

    /// <summary>
    /// Edita uma meta pelo ID informado.
    /// </summary>

    public async Task<bool> PutGoal(int id, UpdateGoalDto dto)
    {
        var goal = await _context.Goals.FindAsync(id);

        if (goal == null)
        {
            return false;
        }

        goal.Title = dto.Title;
        goal.TargetAmount = dto.TargetAmount;
        goal.Deadline = dto.DeadLine;
        goal.Type = dto.Type;

        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Deleta uma meta pelo ID informado.
    /// </summary>
    
    public async Task<bool> DeleteGoal(int id)
    {
        var goal = await _context.Goals.FindAsync(id);

        if (goal == null) 
            return false;

        _context.Goals.Remove(goal);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Retorna a lista completa de metas:
    /// </summary>
    
    public async Task<List<ProgressGoalDto>> GetAllGoals()
    {
        var goals = await _context.Goals
            .Where(g => g.UserId == _userId)
            .ToListAsync();

        var tasks = goals.Select(async g => await GetGoalProgresById(g.ID));

        var results = await Task.WhenAll(tasks);
    
        return results.ToList();
    }

    /// <summary>
    /// Retorna uma meta filtrada por ID.
    /// </summary>
    
    public async Task<ProgressGoalDto> GetGoalProgresById(int id)
    {    
        var goal = await _context.Goals.FindAsync(id);

        if (goal == null)
            throw new KeyNotFoundException("Goal not found");

        var goalBalance = await GetGoalNetBalance(goal);
        decimal progressPercentage = 0;

        if (goal.TargetAmount > 0)
        {
            var actualPercentage = (goalBalance / goal.TargetAmount) * 100m;

            progressPercentage = Math.Clamp(actualPercentage, 0m, 100m);
        }


        return new ProgressGoalDto
        {
            CreateAt = goal.CreatedAt,
            DeadLine = goal.Deadline,
            TargetAmount = goal.TargetAmount,
            Type = goal.Type,
            NetBalance = Math.Round(goalBalance, 2),
            Progress = Math.Round(progressPercentage, 2),
        };
    }

    /// <summary>
    /// Filtra as transações que ocorreram dentro do intervalo de tempo da meta.
    /// </summary>

    public async Task<List<Transaction>> GetGoalTransactions(Goal goal)
    {
        return await _context.Transactions
            .Where(t => t.UserId == _userId)
            .Where(t => t.CreateAt >= goal.CreatedAt.Date)
            .Where(t => t.CreateAt <= goal.Deadline.Date)
            .ToListAsync();
    }

    /// <summary>
    /// Calcula o saldo real da meta subtraindo despesas de receitas.
    /// </summary>

    public async Task<decimal> GetGoalNetBalance(Goal goal)
    {
        List<Transaction> transactions = await GetGoalTransactions(goal);

        var Income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var Expense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        return Income - Expense;
    } 

    /// <summary>
    /// Calcula a porcentagem real de conclusão da meta.
    /// </summary>

    public async Task<decimal> GetGoalProgressPercentage(Goal goal)
    {
        decimal Balance = await GetGoalNetBalance(goal);

        if (goal.TargetAmount == 0) return 0;
        if (Balance > goal.TargetAmount) return 100;
        
        return Balance / goal.TargetAmount * 100m;
    }
}