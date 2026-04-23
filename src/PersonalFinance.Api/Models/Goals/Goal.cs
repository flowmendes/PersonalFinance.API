using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Api.Models.Goals;

/// <summary>
/// Representa uma meta financeira no sistema.
/// </summary>
/// <remarks>
/// Regras de Negócio:
/// <list type="bullet">
/// <item><description>O valor alvo (<see cref="TargetAmount"/>) deve ser estritamente maior que zero.</description></item>
/// <item><description>O tipo (<see cref="Type"/>) determina o impacto da meta no saldo (Economia ou Gasto).</description></item>
/// <item><description>A data <see cref="CreatedAt"/> deve ser sempre registrada em formato UTC para evitar conflitos de fuso horário.</description></item>
/// </list>
/// </remarks>
/// 
public class Goal
{
    [Key]
    public int ID { get; set; }

    [Required]
    [MaxLength(100)]    
    public string Title { get; set; } = string.Empty; // Meta financeira

    [Column(TypeName = "decimal(18,2)")]
    public decimal TargetAmount { get; set; } // Valor alvo
    
    public DateTime Deadline { get; set; } // prazo

    public TypeOfGoal Type { get; set; } // Tipo de Meta
    
    public DateTime CreatedAt { get; set; }
}