using PersonalFinance.Api.Services.Auth;
using PersonalFinance.Api.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace PersonalFinance.Api.Controllers.Auth;

/// <summary>
/// Controller responsável pelo gerencimento de usuarios.
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthServices _authServices;

    public AuthController(IAuthServices authServices)
    {
        _authServices = authServices;
    }

    /// <summary>
    /// Cria uma nova conta de usuário no sistema.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(CreateUserDto dto)
    {
        var createUser = await _authServices.RegisterUser(dto);

        if (createUser == null)
            return BadRequest("Não foi possível realizar o cadastro.");

        return Created();
    }

    /// <summary>
    /// Autentica o usuário e retorna o token de acesso JWT.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var token = await _authServices.Login(email, password);
        
        if (token == null)
            return Unauthorized("E-mail ou senha inválidos.");

        return Ok(new { token });
    }

    /// <summary>
    /// Obtém os detalhes de um usuário específico através do ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = _authServices.GetUserById(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
}