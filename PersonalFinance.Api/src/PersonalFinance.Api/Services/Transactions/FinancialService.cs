using PersonalFinance.Api.Models.Transactions;
using PersonalFinance.Api.DTOs.Transactions;
using PersonalFinance.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinance.Api.Services.Transactions;

public class FinancialService : IFinancialService
{
    private readonly AppDbContext _context;

    public FinancialService(AppDbContext context)
    {
        _context = context;
    }
    

    /// <summary>
    /// Adiciona transação ao banco de dados
    /// </summary>

    public async Task AddTransaction(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Semeia o banco de dados
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
    /// Edita uma transação pelo ID informado.
    /// </summary>
    
    public async Task<bool> PutTransaction(int id, UpdateTransactionDto dto)
    {
        var transaction = await _context.Transactions.FindAsync(id);

        if (transaction == null)
        {
            return false;
        }

        transaction.Description = dto.Description;
        transaction.Amount = dto.Amount;
        transaction.Type = dto.Type;

        await _context.SaveChangesAsync();
                
        return true;
    }

    /// <summary>
    /// Deleta uma transação pelo ID informado.
    /// </summary>

    public async Task<bool> DeleteTransaction(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);

        if (transaction == null)
            return false;

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Retorna a lista de transações com filtros opcionais por:
    /// intervalo de datas, busca por descrição e ordenação.
    /// </summary>

    public async Task <List<Transaction>> GetAllTransactions(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        string? searchTerm = null, 
        string? sortBy = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = _context.Transactions.AsQueryable();

        // Aplica filtro por data inicial (>=)
        if (startDate.HasValue)
            query = query.Where(t => t.CreateAt >= startDate.Value);

        // Aplica filtro por data final (<=)
        if (endDate.HasValue)
            query = query.Where(t => t.CreateAt <= endDate.Value);

        // Aplica filtro por descrição (busca parcial)
        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(t => t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        // Aplica ordenação dinâmica conforme parâmetro informado
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
    ///  Calcula o saldo atual (receita - despesa).
    /// </summary>
    
    public async Task<decimal> GetCurrentBalance()
    {
        var result = await _context.Transactions
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
    /// Retorna o maior valor entre as despesas.
    /// </summary>
    
    public decimal GetBiggestValue()
    {
        var biggestValue = _context.Transactions.Where(t => t.Type == TransactionType.Expense).Max (t => (decimal?)t.Amount) ?? 0;
        
        return biggestValue;
    }

    /// <summary>
    /// Retorna uma Transação filtrada por ID.
    /// </summary>
    
    public async Task<Transaction?> GetTransactionById(int id)
    {
        return _context.Transactions.Find(id);
    }

    /// <summary>
    /// Retorna o resumo financeiro.
    /// </summary>
    
    public async Task<FinancialSummaryDto> GetFinancialSumarry()
    {
        var incomes = _context.Transactions.Where(t => t.Type == TransactionType.Income).Sum (t => t.Amount);
        var expenses = _context.Transactions.Where(t => t.Type == TransactionType.Expense) .Sum (t => t.Amount);

        FinancialSummaryDto summaryDto = new FinancialSummaryDto
        {
            BiggestExpense = GetBiggestValue(),
            TotalExpense = expenses,
            TotalIncomes = incomes,
            CurrentBalance = await GetCurrentBalance()
        };

        return summaryDto;
    }
}


