using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Api.DTOs.Auth;

public class CreateUserDto
{
    [Required]
    [StringLength(40, MinimumLength = 1)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [StringLength(60, MinimumLength = 1)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}