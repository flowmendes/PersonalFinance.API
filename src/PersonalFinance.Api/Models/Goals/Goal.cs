using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Api.Models.Goals;

public class Goal
{
    [Key]
    public int ID { get; set; }

    public String UserId { get ; set; } = string.Empty;

    [Required]
    [MaxLength(100)]    
    public string Title { get; set; } = string.Empty; // Meta financeira

    [Column(TypeName = "decimal(18,2)")]
    public decimal TargetAmount { get; set; } // Valor alvo
    
    public DateTime Deadline { get; set; } // prazo

    public TypeOfGoal Type { get; set; } // Tipo de Meta
    
    public DateTime CreatedAt { get; set; } // Data de criação

    public GoalStatus Status { get; set; } // Status da meta
}