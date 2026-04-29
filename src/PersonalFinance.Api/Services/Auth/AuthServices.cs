using BCrypt.Net;
using PersonalFinance.Api.Models.User;
using PersonalFinance.Api.Data;

namespace PersonalFinance.Api.Services.Auth;

public class AuthServices : IAuthServices
{
    private readonly AppDbContext _context;
    public AuthServices(AppDbContext context)
    {
        _context = context;
    }

    private string ConvertToHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public async Task<User> AddUser(User user, string password)
    {
        user.HashPassword = ConvertToHash(password);
        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }
}