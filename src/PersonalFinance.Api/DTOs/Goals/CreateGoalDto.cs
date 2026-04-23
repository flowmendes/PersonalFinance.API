using PersonalFinance.Api.Models.Goals;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Api.DTOs.Goals;

public class CreateGoalDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 100 caracteres.")]
    public string Title { get; set; } = string.Empty; 

    [Required(ErrorMessage = "Valor alvo é obrigatório")] 
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal TargetAmount { get; set; } 

    [Required(ErrorMessage = "A data de prazo (Deadline) é obrigatória")]
    [DataType(DataType.Date, ErrorMessage = "Formato de data inválido")]
    public DateTime DeadLine { get; set; } 

    [Required(ErrorMessage = "O tipo de meta é obrigatório.")]
    [EnumDataType(typeof(TypeOfGoal), ErrorMessage = "Tipo de meta inválido.")]
    public TypeOfGoal Type { get; set; }

    
}