using PersonalFinance.Api.Models.Transactions;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Api.DTOs.Transactions;

public class CreateTransactionDto
{
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 100 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "O valor da transação é obrigatório")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "O tipo da transação é obrigatório")]
    [EnumDataType(typeof(TransactionType), ErrorMessage = "Tipo de meta inválido.")]
    public TransactionType Type { get; set; }
}