using PersonalFinance.Api.Services.Goals;
using Microsoft.AspNetCore.Authorization;
using PersonalFinance.Api.DTOs.Goals;
using Microsoft.AspNetCore.Mvc;

namespace PersonalFinance.Api.Controllers.Goals;

/// <summary>
/// Controller responsável por gerencia as metas financeiras dos usuários.
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
    /// Lista todas as metas do usuário autenticado.
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllGoal()
    {
        var goalHistrory = await _goalService.GetAllGoals();

        return Ok(goalHistrory);
    }

    /// <summary>
    /// Obtém os detalhes e o progresso de uma meta específica pelo ID.
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
    /// Atualiza as informações de uma meta existente.
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
    /// Cria uma nova meta financeira para o usuário.
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
    /// Remove permanentemente uma meta do sistema.
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