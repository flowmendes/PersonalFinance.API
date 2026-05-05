using PersonalFinance.Api.Data;
using PersonalFinance.Api.Services.Auth;
using PersonalFinance.Api.DTOs.Auth;
using PersonalFinance.Api.Models.Users;
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
    private readonly AppDbContext _context;

    public AuthController(IAuthServices authServices, AppDbContext context)
    {
        _authServices = authServices;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(CreateUserDto dto)
    {
        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
        };

        var registerUser = await _authServices.RegisterUser(user, dto.Password);
        return Created("", registerUser);
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