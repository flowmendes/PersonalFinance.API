using PersonalFinance.Api.Models.Transactions;
using PersonalFinance.Api.DTOs.Transactions;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Api.Data;
using System.Security.Claims;

namespace PersonalFinance.Api.Services.Transactions;

public class FinancialService : IFinancialService
{
    private readonly AppDbContext _context;
    private readonly string? _userId;

    public FinancialService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;

        _userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
    
    /// <summary>
    /// Registra uma nova transação financeira vinculada ao usuário autenticado.
    /// </summary>
    public async Task<Transaction> AddTransaction(CreateTransactionDto dto)
    {
        if (string.IsNullOrEmpty(_userId))
            throw new UnauthorizedAccessException("Unidentified user");
        

        if (dto.GoalId.HasValue)
            await ValidGoal(dto.GoalId.Value);

        var createTransaction = new Transaction
        {
            UserId = _userId,
            Description = dto.Description,
            Amount = dto.Amount,
            Type = dto.Type,
            CreateAt = DateTime.UtcNow
        };

        await _context.Transactions.AddAsync(createTransaction);
        await _context.SaveChangesAsync();

        return createTransaction;
    }

    /// <summary>
    /// Gera dados fictícios para testes de performance e paginação.
    /// </summary>
    public async Task SeedData(AppDbContext context)
    {
        var transactions = new List<Transaction>();

        for (int i = 0; i < 1000; i++)
        {
            int number = Random.Shared.Next(1, 3);

            transactions.Add(new Transaction
            {
                Description = $"Transaction {i}",
                Amount = Random.Shared.Next(1, 10000),
                CreateAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 365)),
                Type = (TransactionType) number
            });
        }

        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Atualiza os dados de uma transação, validando se ela pertence ao usuário logado.
    /// </summary>
    public async Task<bool> PutTransaction(int id, UpdateTransactionDto dto)
    {
        var transaction = await _context.Transactions.FindAsync(id);

        if (transaction == null || transaction.UserId != _userId)
            return false;

        transaction.Description = dto.Description;
        transaction.Amount = dto.Amount;
        transaction.Type = dto.Type;

        await _context.SaveChangesAsync();
                
        return true;
    }

    /// <summary>
    /// Remove uma transação do banco de dados após validar a permissão do usuário.
    /// </summary>
    public async Task<bool> DeleteTransaction(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);

        if (transaction == null)
            return false;
        
        if (transaction.UserId != _userId)
            return false;

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Retorna o histórico de transações com suporte a filtros, busca, ordenação e paginação.
    /// </summary>
    public async Task <List<Transaction>> GetAllTransactions(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        string? searchTerm = null, 
        string? sortBy = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = _context.Transactions
        .Where(t => t.UserId == _userId)
        .AsQueryable();

        // Filtro de datas
        if (startDate.HasValue)
            query = query.Where(t => t.CreateAt >= startDate.Value); // Aplica filtro por data inicial (>=)

        if (endDate.HasValue)
            query = query.Where(t => t.CreateAt <= endDate.Value); // Aplica filtro por data final (<=)

        // Busca textual parcial na descrição
        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(t => t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        // Lógica de ordenação dinâmica
        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "amount_desc":
                    query = query.OrderByDescending(t => t.Amount);
                    break;

                case "amount_asc":
                    query = query.OrderBy(t => t.Amount);
                    break;

                case "date_desc":
                    query = query.OrderByDescending(t => t.CreateAt);
                    break;
                
                case "date_asc":
                    query = query.OrderBy(t => t.CreateAt);
                    break;
                
                default:
                    query = query.OrderByDescending(t => t.CreateAt);
                    break;
            }
        }
        else
        {
            // Ordenação padrão quando nenhum critério é informado
            query = query.OrderByDescending(t => t.CreateAt);
        }

        // Paginação
        query = query 
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return await query.ToListAsync();
    }

    /// <summary>
    ///  Calcula o saldo líquido total (Entradas - Saídas) do usuário.
    /// </summary>
    public async Task<decimal> GetCurrentBalance()
    {
        var result = await _context.Transactions
            .Where(g => g.UserId == _userId)
            .GroupBy(t => t.Type)
            .Select(g => new {
                Type = g.Key,
                Total = g.Sum(t => t.Amount)
        })
        .ToListAsync();

        var income = result.FirstOrDefault(x => x.Type == TransactionType.Income)?.Total ?? 0;
        var expense = result.FirstOrDefault(x => x.Type == TransactionType.Expense)?.Total ?? 0;

        return income - expense;
    }

    /// <summary>
    /// Identifica o maior valor de despesa registrado pelo usuário.
    /// </summary>
    public async Task<decimal> GetBiggestValue()
    {
        // Uso de MaxAsync para evitar bloqueio de thread
        return await _context.Transactions
            .Where(t => t.UserId == _userId && t.Type == TransactionType.Expense)
            .MaxAsync(t => (decimal?)t.Amount) ?? 0;
    }

    /// <summary>
    /// Busca uma transação específica validando o isolamento de dados por usuário.
    /// </summary>
    public async Task<Transaction?> GetTransactionById(int id)
    {
        var result = _context.Transactions.Find(id);

        if (result == null || result.UserId != _userId)
            return null;
        
        return result;
    }

    /// <summary>
    /// Consolida as estatísticas financeiras gerais em um objeto de resumo.
    /// </summary>
    public async Task<FinancialSummaryDto> GetFinancialSumarry()
    {
        // Busca os totais para evitar múltiplas chamadas ao banco
        var result = await _context.Transactions
            .Where(g => g.UserId == _userId)
            .GroupBy(t => t.Type)
            .Select(g => new {
                Type = g.Key,
                Total = g.Sum(t => t.Amount)
        })
        .ToListAsync();

        var incomes = result.FirstOrDefault(x => x.Type == TransactionType.Income)?.Total ?? 0;
        var expenses = result.FirstOrDefault(x => x.Type == TransactionType.Expense)?.Total ?? 0;

        FinancialSummaryDto summaryDto = new FinancialSummaryDto
        {
            BiggestExpense = await GetBiggestValue(),
            TotalExpense = expenses,
            TotalIncomes = incomes,
            CurrentBalance = incomes - expenses
        };

        return summaryDto;
    }

    /// <summary>
    /// Busca uma meta enviada pelo usuário, caso seja invalida retorna erro.
    /// </summary>
    private async Task<bool> ValidGoal(int id)
    {
        var goal = await _context.Goals.FindAsync(id);

        if (goal == null || goal.UserId != _userId)
            throw new KeyNotFoundException("A meta informada não foi encontrada ou não pertence a este usuário.");

        if (goal.Status == Models.Goals.GoalStatus.Finished || goal.Status == Models.Goals.GoalStatus.Canceled)
            throw new InvalidOperationException("Não é possível vincular transações a uma meta concluída ou cancelada.");

        return true;
    }

    private async Task<bool> DepositGoal (int id, decimal value)
    {
        var goal = await _context.Goals.FindAsync(id);

        goal.CurrentValue = value;
    }
}