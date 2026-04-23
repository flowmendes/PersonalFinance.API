
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;

namespace PersonalFinance.Api.Models.User;

public class User()
{
    [Key]
    public int Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
};