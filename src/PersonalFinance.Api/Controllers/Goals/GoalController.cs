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
            return NotFound(new { message = "Meta não encontrada ou acesso negado." });

        return Ok(goal);
    }

    /// <summary>
    /// Atualiza as informações de uma meta existente.
    /// </summary> 
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGoal(int id, UpdateGoalDto dto)
    {
        var goal = await _goalService.PutGoal(id, dto);

        if (!goal)
            return NotFound(new { message = "Meta não encontrada ou acesso negado." });
        
        return NoContent();
    }

    /// <summary>
    /// Cancela a meta e altera o status para (Canceled).
    /// </summary>
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelGoal(int id)
    {
        var result = await _goalService.CancelGoal(id);

        if (!result)
            return NotFound(new { message = "Meta não encontrada ou acesso negado." });
            
        return NoContent();
    }

    /// <summary>
    /// Pausa a meta e altera o status para (Paused).
    /// </summary>
    [HttpPut("{id}/pause")]
    public async Task<IActionResult> PauseGoal(int id)
    {
        var result = await _goalService.PauseGoal(id);
        if (!result)
            return NotFound(new { message = "Meta não encontrada ou acesso negado." });
        
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

        if(!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> TestGetStatus(int id)
    {
        var result = await _goalService.TestGetStatus(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}