using System.ComponentModel.DataAnnotations;

namespace MarketGossip.ChatApp.Application.Features.Authentication.Models;

public class LoginModel
{
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}