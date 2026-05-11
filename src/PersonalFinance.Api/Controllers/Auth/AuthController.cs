using PersonalFinance.Api.Services.Auth;
using PersonalFinance.Api.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace PersonalFinance.Api.Controllers.Auth;

/// <summary>
/// Controller responsável por gerenciar os usuarios da API.
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

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(CreateUserDto dto)
    {
        var createUser = await _authServices.RegisterUser(dto);

        return Created("", createUser);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var token = await _authServices.Login(email, password);
        
        if (token == null)
            return NotFound();

        return Ok(new { token });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = _authServices.GetUserById(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
}