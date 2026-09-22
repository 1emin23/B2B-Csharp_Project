using System.Security.Claims;
using C1Soft.Business.DTOs.Auth;
using C1Soft.Business.Interfaces;
using C1Soft.DataAccess.Context;
using C1Soft.DataAccess.Security;
using C1Soft.Domain.Entities;
using C1Soft.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace C1Soft.Business.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(AppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<ClaimsPrincipal?> LoginAsync(LoginDto dto)
    {
        // SQL seviyesinde kullanıcı adı VEYA e-posta ile ara (case-insensitive)
        var user = await _context.Users
            .Where(u => u.IsActive &&
                        (u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail))
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.FirstName,
                u.LastName,
                u.Role,
                u.PasswordHash,
                u.PasswordSalt,
                u.IsActive
            })
            .FirstOrDefaultAsync();

        if (user is null)
            return null;

        if (!_passwordHasher.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
            return null;

        // Cookie Authentication için claims oluştur
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}".Trim()),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        return new ClaimsPrincipal(identity);
    }

    public async Task<int> RegisterAsync(RegisterDto dto)
    {
        // Unique kontrolü — SQL sorgusu ile
        var isDuplicate = await _context.Users
            .AnyAsync(u => u.Email == dto.Email || u.Username == dto.Username);

        if (isDuplicate)
            throw new InvalidOperationException("Bu e-posta veya kullanıcı adı zaten kullanılmaktadır.");

        _passwordHasher.CreatePasswordHash(dto.Password, out string hash, out string salt);

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Username = dto.Username,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = UserRole.Customer,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    public Task<bool> IsEmailTakenAsync(string email) =>
        _context.Users.AnyAsync(u => u.Email == email);

    public Task<bool> IsUsernameTakenAsync(string username) =>
        _context.Users.AnyAsync(u => u.Username == username);
}
