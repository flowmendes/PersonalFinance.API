using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Api.Models.Transactions;

/// <summary>
/// Representa uma transação financeira do sistema.
/// Uma transação pode ser do tipo Receita ou Despesa e impacta diretamente o saldo.
/// </summary>
/// <remarks>
/// Regras:
/// - O valor (Amount) deve ser maior que zero.
/// - O tipo (Type) define se o valor aumenta ou reduz o saldo.
/// - A data de criação deve ser registrada em UTC.
/// </remarks>

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime CreateAt { get; set; }

    public TransactionType Type { get; set; }
}