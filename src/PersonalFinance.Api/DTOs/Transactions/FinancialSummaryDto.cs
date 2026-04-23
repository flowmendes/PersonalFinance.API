namespace PersonalFinance.Api.DTOs.Transactions;

public class FinancialSummaryDto
{
    public decimal CurrentBalance { get; set; }
    public decimal TotalIncomes { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal BiggestExpense { get; set; }
}