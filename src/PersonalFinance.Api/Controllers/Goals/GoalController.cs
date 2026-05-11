using PersonalFinance.Api.Services.Goals;
using Microsoft.AspNetCore.Authorization;
using PersonalFinance.Api.DTOs.Goals;
using Microsoft.AspNetCore.Mvc;

namespace PersonalFinance.Api.Controllers.Goals;

/// <summary>
/// Controller responsável por gerenciar as metas financeiras da API.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GoalController : ControllerBase
{
    private readonly IGoalServices _goalService;

    public GoalController(IGoalServices goalServices)
    {
        _goalService = goalServices;
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

        if (goal == null)
            return NotFound();

        return Ok(goal);
    }

    /// <summary>
    /// Atualiza uma meta existente pelo ID.
    /// </summary>
    
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGoal(int id, UpdateGoalDto dto)
    {
        var goal = await _goalService.PutGoal(id, dto);

        if (goal == false)
            return NotFound();
        
        return NoContent();
    }

    /// <summary>
    /// Adiciona uma nova meta financeira.
    /// </summary> 

    [HttpPost("goal")]
    public async Task<IActionResult> PostGoal(CreateGoalDto dto)
    {
        var createdGoal = await _goalService.AddGoal(dto);

        if (createdGoal == null)
            return BadRequest("Não foi possível criar a meta.");

        return Created("", createdGoal);
    }

    /// <summary>
    /// Exclui uma meta pelo ID.
    /// </summary>

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGoalById(int id)
    {
        var deleted = await _goalService.DeleteGoal(id);

        if(deleted == false)
            return NotFound();

        return NoContent();
    }
}