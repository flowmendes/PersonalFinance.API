
namespace PersonalFinance.Api.Services.Auth;
using PersonalFinance.Api.Models.User;

public interface IAuthServices
{
    public Task<User> AddUser(User user, string password);
}