using PersonalFinance.Api.Models.Transactions;

namespace PersonalFinance.Api.DTOs.Transactions;

public class UpdateTransactionDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
}