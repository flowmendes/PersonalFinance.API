using PersonalFinance.Api.Models.Goals;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Api.DTOs.Goals;

public class UpdateGoalDto
{
    public string Title { get; set; } = string.Empty; 
    public decimal TargetAmount { get; set; } 
    public DateTime DeadLine { get; set; } 
    public TypeOfGoal Type { get; set; }
}