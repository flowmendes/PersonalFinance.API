using BCrypt.Net;

namespace PersonalFinance.Api.Services.Auth;

public class UserServices
{
    public static string ConvertToHash(string password)
    {
        string hash = BCrypt.Net.BCrypt.HashPassword(password);

        return hash;
    }
}