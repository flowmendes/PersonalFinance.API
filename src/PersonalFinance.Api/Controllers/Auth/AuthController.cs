using PersonalFinance.Api.Data;
using PersonalFinance.Api.Services.Auth;
using PersonalFinance.Api.DTOs.User;
using PersonalFinance.Api.Models.User;
using Microsoft.AspNetCore.Mvc;


namespace PersonalFinance.Api.Controllers.Auth;


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

    public async Task<IActionResult> RegisterUser(CreateUserDto dto)
    {
        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
        };

        var registerUser = await _authServices.AddUser(user, dto.Password);
        return Created("", registerUser);
    }
}