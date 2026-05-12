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
    // Recupera o ID do usuário autenticado a partir do Token JWT (Claims)
    private readonly string? _userId;

    public AuthServices(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;

        _userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Gera um hash seguro para a senha utilizando o algoritmo BCrypt.
    /// </summary> 
    private static string ConvertToHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Realiza o cadastro de um novo usuário e armazena a senha criptografada.
    /// </summary>
    public async Task<User> RegisterUser(CreateUserDto dto)
    {
        var guid = Guid.NewGuid().ToString();

        var createUser = new User
        {
            UserId = guid,
            UserName = dto.UserName,
            Email = dto.Email,
            HashPassword = ConvertToHash(dto.Password) // A senha nunca é salva em texto puro.
        };

        _context.Users.Add(createUser);
        await _context.SaveChangesAsync();

        return createUser;
    }

    /// <summary>
    /// Valida as credenciais do usuário e retorna um token JWT se os dados estiverem corretos. 
    /// </summary>
    /// <returns>Token JWT como string ou null caso a autenticação falhe.</returns>
    public async Task<string?> Login(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return null;

        // Compara a senha enviada com o hash armazenado no banco
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.HashPassword);

        if (!isPasswordValid)
            return null;
        
        var token = GenerateToken(user.Email, user.UserId);

        return token;
    }

    /// <summary>
    /// Cria um token JWT contendo as informações de identidade do usuário (Claims).
    /// </summary>
    /// <param name="email">E-mail do usuário autenticado.</param>
    /// <param name="userId">ID único do usuário no banco de dados.</param>
    public string GenerateToken(string email, string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes("Minha_Chave_Ultra_Secreta_Com_Mais_De_32_Caracteres_Esta_Aqui");

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId) // Importante para o filtro de dados do usuário
            }),
            Expires = DateTime.UtcNow.AddHours(3), // Define validade do acesso
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescription);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Busca um usuário específico no banco de dados através do ID.
    /// </summary>
    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }
}