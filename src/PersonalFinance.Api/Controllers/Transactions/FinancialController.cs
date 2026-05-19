using PersonalFinance.Api.Services.Transactions;
using PersonalFinance.Api.DTOs.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Api.Data;

namespace PersonalFinance.Api.Controllers.Transactions;

/// <summary>
/// Controller responsável por gerencia as movimentações financeiras, saldos e histórico do usuário.
/// </summary>

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FinancialController : ControllerBase
{
    private readonly IFinancialService _service;
    private readonly AppDbContext _context;

    public FinancialController(IFinancialService service, AppDbContext context)
    {
        _service = service;
        _context = context;
    }

    /// <summary>
    /// Calcula o saldo líquido total do usuário logado.
    /// </summary>
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance() 
    {
        var balance = await _service.GetCurrentBalance();
        return Ok(balance);
    }

    /// <summary>
    /// Lista o histórico de transações com suporte a filtros de data e paginação.
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetHistory([FromQuery] DateTime? start, [FromQuery] DateTime? end, int pageNumber = 1, int pageSize = 10)
    {
        var history = await _service.GetAllTransactions(start, end, null, null, pageNumber, pageSize);

        return Ok(history);
    }

    /// <summary>
    /// Recupera o valor da maior despesa individual registrada.
    /// </summary>
    [HttpGet("biggest-expense")]
    public IActionResult GetBiggestExpense()
    {
        var value = _service.GetBiggestValue();
        return Ok(value);
    }

    /// <summary>
    /// Obtém os detalhes de uma transação específica através do ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var transaction = await _service.GetTransactionById(id);

        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }

    /// <summary>
    /// Retorna um resumo consolidado de receitas, despesas e saldo
    /// </summary>
    [HttpGet("financial-summary")]
    public IActionResult GetFinancialSumarry()
    {
        var summary = _service.GetFinancialSumarry();

        if (summary == null)
            return NotFound();

        return Ok(summary);
    }

    /// <summary>
    /// Registra uma nova entrada ou saída financeira.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostTransaction(CreateTransactionDto dto)
    {
        try
        {
            var created = await _service.AddTransaction(dto);

            return Created(string.Empty, created);
        } 
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "A meta informada não foi encontrada ou não pertence a este usuário."});
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Usuário não autenticado ou sessão expirada." });
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new { message = "Não é possível vincular transações a uma meta concluída ou cancelada."});
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Erro interno ao salvar a transação." });
        }
    }

    /// <summary>
    /// Popula o banco de dados com dados iniciais de teste (Apenas Desenvolvimento).
    /// </summary>
    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        await _service.SeedData(_context);

        return Ok("Dados semeados");
    }

    /// <summary>
    /// Altera os dados de uma transação existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTransactionDto dto)
    {
        var updated = await _service.PutTransaction(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Remove permanentemente uma transação do histórico.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var deleted = await _service.DeleteTransaction(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}