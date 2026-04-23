using PersonalFinance.Api.DTOs.Goals;
using PersonalFinance.Api.Models.Goals;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Api.Data;
using PersonalFinance.Api.Services.Goals;

namespace PersonalFinance.Api.Controllers.Goals;

/// <summary>
/// Controller responsável por gerenciar as metas financeiras da API.
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class GoalController : ControllerBase
{
    private readonly IGoalServices _goalService;
    private readonly AppDbContext _context;

    public GoalController(IGoalServices goalServices, AppDbContext context)
    {
        _goalService = goalServices;
        _context = context;
    }


    /// <summary>
    /// Retorna todas as metas criadas
    /// </summary>

    [HttpGet("all")]
    public async Task<IActionResult> GetAllGoal()
    {
        var goalHistrory = await _goalService.GetAllGoals();
        return Ok(goalHistrory);
    }

    /// <summary>
    /// Retorna uma meta filtrada pelo ID
    /// </summary>

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGoalById(int id)
    {
        var goal = await _goalService.GetGoalProgresById(id);

        return Ok(goal);
    }

    /// <summary>
    /// Atualiza uma meta existente pelo ID.
    /// </summary>
    
    [HttpPut]
    public async Task<IActionResult> PutGoal(int id, UpdateGoalDto dto)
    {
        var goal = await _goalService.PutGoal(id, dto);

        if (!goal)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Adiciona uma nova meta financeira.
    /// </summary> 

    [HttpPost("goal")]
    public async Task<IActionResult> PostGoal(CreateGoalDto dto)
    {
        
        var goal = new Goal
        {
            Title = dto.Title,
            TargetAmount = dto.TargetAmount,
            Deadline = dto.DeadLine,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };

        var createdGoal = await _goalService.AddGoal(goal);
        return Created("", createdGoal);
    }

    /// <summary>
    /// Exclui uma meta pelo ID.
    /// </summary>

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGoalById(int id)
    {
        var deleted = await _goalService.DeleteGoal(id);

        if(!deleted)
            return NotFound();

        return NoContent();
    }
}