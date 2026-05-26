using PersonalFinance.Api.Models.Goals;
using PersonalFinance.Api.DTOs.Goals;

namespace PersonalFinance.Api.Services.Goals;
public interface IGoalServices
{
    Task<Goal?> AddGoal(CreateGoalDto dto);
    Task<List<ProgressGoalDto?>> GetAllGoals();
    Task<ProgressGoalDto?> GetGoalProgresById(int id);
    Task<bool> DeleteGoal(int id);
    Task<bool> PutGoal(int id, UpdateGoalDto dto);
    Task<bool> PauseGoal(int Id);
    Task<bool> CancelGoal(int Id);
    Task<GoalStatus?> TestGetStatus(int id);
}