namespace HealthentiaVitalsDashboard.Models;
using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}