using PersonalFinance.Api.Data;
using PersonalFinance.Api.Models.Transactions;
using PersonalFinance.Api.DTOs.Transactions;

namespace PersonalFinance.Api.Services.Transactions;

public interface IFinancialService
{
    Task AddTransaction(Transaction transaction);
    Task SeedData(AppDbContext context);
    Task<List<Transaction>> GetAllTransactions(
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? searchTerm = null,
        string? sortBy = null,
        int pageNumber = 1,
        int pageSize = 10);
    Task<Transaction?> GetTransactionById(int id);
    Task<FinancialSummaryDto> GetFinancialSumarry();
    Task<decimal> GetCurrentBalance();
    decimal GetBiggestValue();
    Task<bool> DeleteTransaction(int id);
    Task<bool> PutTransaction(int id, UpdateTransactionDto dto);
}