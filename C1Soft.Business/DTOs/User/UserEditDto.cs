using C1Soft.Domain.Enums;

namespace C1Soft.Business.DTOs.User;

public class UserEditDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// Boş bırakılırsa şifre değiştirilmez.
    /// </summary>
    public string? NewPassword { get; set; }
}
