using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinance.Api.Models.RecurringPayment;
public class RecurringPayment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]  
    public string Description { get; set; }  = string.Empty; // Ex: "Netflix, Prime video, HBO"

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; } // Valor da assinatura
    public int DueDay { get; set; } // Dia do vencimento
    public string Status { get; set; } = string.Empty; // status 
    
    [Required]
    public string UserId { get; set; } = string.Empty; // "Pending", "Paid", "Failed"
}