using System.Security.Claims;
using C1Soft.Business.DTOs.Auth;

namespace C1Soft.Business.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Kullanıcı adı veya e-posta + şifre ile giriş yapar.
    /// Başarılıysa Cookie Auth için ClaimsPrincipal döner.
    /// </summary>
    Task<ClaimsPrincipal?> LoginAsync(LoginDto dto);

    /// <summary>
    /// Yeni bayi (Customer) hesabı oluşturur.
    /// Başarılıysa oluşturulan kullanıcının Id'sini döner.
    /// Hata durumunda exception fırlatır.
    /// </summary>
    Task<int> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// E-posta veya kullanıcı adının sistemde kayıtlı olup olmadığını kontrol eder.
    /// </summary>
    Task<bool> IsEmailTakenAsync(string email);
    Task<bool> IsUsernameTakenAsync(string username);
}
