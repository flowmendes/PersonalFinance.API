using PersonalFinance.Api.Models.Transactions;
using PersonalFinance.Api.DTOs.Transactions;
using PersonalFinance.Api.Services.Transactions;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Api.Data;

namespace PersonalFinance.Api.Controllers.Transactions;

/// <summary>
/// Controller responsável por gerenciar as transações financeiras da API.
/// </summary>

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
    /// Retorna o saldo atual do usuário (receitas - despesas).
    /// </summary>

    [HttpGet("balance")]
    public IActionResult GetBalance()
    {
        var balance = _service.GetCurrentBalance();
        return Ok(balance);
    }

    /// <summary>
    /// Retorna todas as transações, podendo filtrar por intervalo de datas.
    /// </summary>

    [HttpGet("all")]
    public async Task<IActionResult> GetHistory([FromQuery] DateTime? start, [FromQuery] DateTime? end, int pageNumber = 1, int pageSize = 10)
    {
        var history = await _service.GetAllTransactions(start, end, null, null, pageNumber, pageSize);

        return Ok(history);
    }

    /// <summary>
    /// Retorna o maior valor de despesa registrado.
    /// </summary>

    [HttpGet("biggest-expense")]
    public IActionResult GetBiggestExpense()
    {
        var value = _service.GetBiggestValue();
        return Ok(value);
    }

    /// <summary>
    /// Retorna uma transação filtrada pelo ID
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
    /// Retorna o resumo financeiro.
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
    /// Adiciona uma nova transação financeira.
    /// </summary>

    [HttpPost("transaction")]
    public async Task<IActionResult> PostTransaction(CreateTransactionDto dto)
    {
        var transaction = new Transaction
        {
            Description = dto.Description,
            Amount = dto.Amount,
            Type = dto.Type,
            CreateAt = DateTime.UtcNow
        };

        await _service.AddTransaction(transaction);

        return Created("", transaction);
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        await _service.SeedData(_context);

        return Ok("Dados semeados");
    }

    /// <summary>
    /// Atualiza uma transação existente pelo ID.
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
    /// Exclui uma transação pelo ID.
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