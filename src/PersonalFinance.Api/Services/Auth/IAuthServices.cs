using PersonalFinance.Api.Models.Users;
using PersonalFinance.Api.DTOs.Auth;

namespace PersonalFinance.Api.Services.Auth;

public interface IAuthServices
{
    public Task<User> RegisterUser(CreateUserDto dto);
    public Task<string?> Login(string email, string password);
    public Task<User?> GetUserById(int id);
}