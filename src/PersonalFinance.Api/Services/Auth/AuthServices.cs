using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PersonalFinance.Api.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Api.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using PersonalFinance.Api.Data;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace PersonalFinance.Api.Services.Auth;

public class AuthServices : IAuthServices
{
    private readonly AppDbContext _context;
    private readonly string? _userId;

    public AuthServices(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;

        _userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Converete a senha em hash
    /// </summary>
    
    private static string ConvertToHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Registra novo usuario
    /// </summary>

    public async Task<User> RegisterUser(CreateUserDto dto)
    {
        var guid = Guid.NewGuid().ToString();

        var createUser = new User
        {
            UserId = guid,
            UserName = dto.UserName,
            Email = dto.Email,
            HashPassword = ConvertToHash(dto.Password)
        };

        _context.Users.Add(createUser);
        await _context.SaveChangesAsync();

        return createUser;
    }

    /// <summary>
    /// Valida login de usuario 
    /// </summary>

    public async Task<string?> Login(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return null;

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.HashPassword);

        if (!isPasswordValid)
            return null;
        
        var token = GenerateToken(user.Email, user.UserId);

        return token;
    }

    public string GenerateToken(string email, string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes("Minha_Chave_Ultra_Secreta_Com_Mais_De_32_Caracteres_Esta_Aqui");

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId)
            }),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescription);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Retorna um usuario filtrada pelo ID
    /// </summary>

    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }
}