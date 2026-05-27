using PersonalFinance.Api.Models.Transactions;
using PersonalFinance.Api.Models.Goals;
using PersonalFinance.Api.DTOs.Goals;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Api.Data;
using System.Security.Claims;
using System.Data;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;

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
    /// Cria uma nova meta financeira vinculada ao usuário autenticado.
    /// </summary>
    public async Task<Goal?> AddGoal(CreateGoalDto dto)
    {
        if (string.IsNullOrEmpty(_userId))
            return null;
        
        var createGoal = new Goal
        {
            UserId = _userId,
            Title = dto.Title,
            TargetAmount = dto.TargetAmount,
            Deadline = dto.DeadLine,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow, // Define o início da contagem para o progresso da meta
            Status = GoalStatus.Pending  // Define o status inícial da meta
        };

        _context.Goals.Add(createGoal);
        await _context.SaveChangesAsync();

        return createGoal;
    }

    /// <summary>
    /// Atualiza os dados de uma meta existente, validando a propriedade do usuário.
    /// </summary>
    public async Task<bool> PutGoal(int id, UpdateGoalDto dto)
    {
        var goal = await _context.Goals.FindAsync(id);

        // Garante que a meta existe e pertence ao usuário logado
        if (ValidGoal(goal)) 
            return false;
        
        goal!.Title = dto.Title;
        goal.TargetAmount = dto.TargetAmount;
        goal.Deadline = dto.DeadLine;
        goal.Type = dto.Type;

        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Remove uma meta do banco de dados.
    /// </summary>
    public async Task<bool> DeleteGoal(int id)
    {
        var goal = await _context.Goals.FindAsync(id);

        if (ValidGoal(goal)) 
            return false;

        _context.Goals.Remove(goal!);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Recupera todas as metas do usuário, incluindo os cálculos de progresso atualizados.
    /// </summary>
    public async Task<List<ProgressGoalDto?>> GetAllGoals()
    {
        var goals = await _context.Goals
            .Where(g => g.UserId == _userId)
            .ToListAsync();

        // Executa o processamento de progresso para cada meta de forma assíncrona
        var tasks = goals.Select(async g => await GetGoalProgresById(g.ID));
        var results = await Task.WhenAll(tasks);
    
        return results.ToList();
    }

    /// <summary>
    /// Retorna os detalhes de uma meta específica e calcula o percentual de conclusão.
    /// </summary>
    public async Task<ProgressGoalDto?> GetGoalProgresById(int id)
    {    
        var goal = await _context.Goals.FindAsync(id);

        if (goal == null || goal.UserId != _userId) 
            return null;

        var goalBalance = await GetGoalNetBalance(goal);
        decimal progressPercentage = 0;

        // Regra de Negócio: O progresso é a relação entre o saldo líquido do período e o valor alvo
        if (goal.TargetAmount > 0)
        {
            var actualPercentage = (goalBalance / goal.TargetAmount) * 100m;
            // Garante que o progresso visual fique entre 0% e 100%
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
            Status = goal.Status
        };
    }

    /// <summary>
    /// Busca todas as transações realizadas entre a criação e o prazo final da meta.
    /// </summary>
    public async Task<List<Transaction>> GetGoalTransactions(Goal goal)
    {
        return await _context.Transactions
            .Where(t => t.UserId == _userId)
            .Where(t => t.CreateAt >= goal.CreatedAt.Date)
            .Where(t => t.CreateAt <= goal.Deadline.Date)
            .Where(t => t.GoalId == goal.ID)
            .ToListAsync();
    }

    /// <summary>
    /// Realiza o cálculo e define o status atual da meta.
    /// </summary>
    public async Task<GoalStatus?> GetGoalStatus(int Id)
    {
        var result = await _context.Goals.FindAsync(Id);

        if (result == null || result.UserId != _userId)
            return null;

        return result.Status;
    }

    /// <summary>
    /// Altera o status atual da meta para Canceled
    /// </summary>
    public async Task<bool> CancelGoal(int Id)
    {
        var goal = await _context.Goals.FindAsync(Id);

        if (goal == null || goal.UserId != _userId)
            return false;

        if (goal.Status == GoalStatus.Canceled)
            return true;

        goal.Status = GoalStatus.Canceled;

        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Altera o status atual da meta para Paused.
    /// </summary>
    public async Task<bool> PauseGoal(int Id)
    {
        var goal = await _context.Goals.FindAsync(Id);

        if (ValidGoal(goal) == false)
            return false;

        if (goal!.Status == GoalStatus.Paused)
            return true;

        goal.Status = GoalStatus.Paused;

        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Realiza o cálculo do saldo líquido (Receitas - Despesas) dentro do período da meta.
    /// </summary>
    public async Task<decimal> GetGoalNetBalance(Goal goal)
    {
        // Faz a conta direto no banco de dados e traz apenas o número final para a memória
        var netBalance = await _context.Transactions
            .Where(t => t.UserId == _userId && t.GoalId == goal.ID)
            .SumAsync(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);

        // Garante que o saldo nunca seja negativo
        return Math.Max(0m, netBalance);
    } 

    /// <summary>
    /// Método auxiliar para cálculo direto de porcentagem de progresso.
    /// </summary>
    public async Task<decimal> GetGoalProgressPercentage(Goal goal)
    {
        decimal balance = await GetGoalNetBalance(goal);

        if (goal.TargetAmount <= 0) 
            return 0;

        var persentage = balance / goal.TargetAmount * 100m;

        // Se o progresso atingiu ou passou de 100%, verificamos a atualização do status
        if (balance >= goal.TargetAmount)
        {
            await UpdateGoalStatusIfFinished(goal, persentage);
            return 100;
        }
        // Retorna a porcentagem real (ou 0 caso o saldo seja negativo)
        return Math.Max(0, persentage);
    }

    /// <summary>
    /// Verifica e atualiza o status da meta com base no progresso atual.
    /// </summary>
    public async Task UpdateGoalStatusIfFinished(Goal goal, decimal TargetAmount)
    {
        if (TargetAmount >= 100 && goal.Status != GoalStatus.Finished)
        {
            goal.Status = GoalStatus.Finished;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Verifica se a meta e valida para uso.
    /// </summary>
    private bool ValidGoal(Goal? goal)
    {
        // 1. Se a meta não existe
        if (goal == null)
            return false;
        
        // 2. Se a meta não pertence ao usuário logado
        if (goal.UserId != _userId)
            return false;

        // 3. Se a meta já foi explicitamente encerrada (Cancelada ou Concluída)
        if (goal.Status == GoalStatus.Canceled || goal.Status == GoalStatus.Finished)
            return false;

        // 4. Se o status já estiver salvo como Overdue OU se o prazo já passou do momento atual (Atrasada)
        if (goal.Status == GoalStatus.Overdue || goal.Deadline < DateTime.Now)
            return false;

        return true; // Meta encontrada, do usuário, aberta e dentro do prazo!
    }
}