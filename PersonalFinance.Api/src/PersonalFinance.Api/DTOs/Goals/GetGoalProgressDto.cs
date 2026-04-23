using PersonalFinance.Api.Models.Goals;

namespace PersonalFinance.Api.DTOs.Goals;

public class ProgressGoalDto
{
    public string Title { get; set; } = string.Empty; 
    public decimal TargetAmount { get; set; } 
    public DateTime DeadLine { get; set; } 
    public TypeOfGoal Type { get; set; }
    public DateTime CreateAt { get; set; }
    public decimal Progress { get; set; } 
    public decimal NetBalance { get; set; }
}
