using C1Soft.Business.DTOs.User;
using C1Soft.Business.Interfaces;
using C1Soft.DataAccess.Context;
using C1Soft.DataAccess.Security;
using Microsoft.EntityFrameworkCore;

namespace C1Soft.Business.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(AppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserListDto>> GetAllUsersAsync(string? searchQuery)
    {
        // Admin pasif kullanıcıları da görebilmeli
        var query = _context.Users
            .IgnoreQueryFilters()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var q = searchQuery.Trim();
            query = query.Where(u =>
                EF.Functions.Like(u.FirstName, $"%{q}%") ||
                EF.Functions.Like(u.LastName, $"%{q}%") ||
                EF.Functions.Like(u.Username, $"%{q}%") ||
                EF.Functions.Like(u.Email, $"%{q}%"));
        }

        return await query
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserListDto
            {
                Id = u.Id,
                FullName = u.FirstName + " " + u.LastName,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<UserEditDto?> GetUserForEditAsync(int id)
    {
        return await _context.Users
            .IgnoreQueryFilters()
            .Where(u => u.Id == id)
            .Select(u => new UserEditDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Username = u.Username,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                IsActive = u.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateUserAsync(UserEditDto dto)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == dto.Id)
            ?? throw new InvalidOperationException($"Kullanıcı bulunamadı. Id: {dto.Id}");

        // E-posta/kullanıcı adı çakışma kontrolü (başkasının kaydı ile)
        var isDuplicate = await _context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Id != dto.Id &&
                           (u.Email == dto.Email || u.Username == dto.Username));

        if (isDuplicate)
            throw new InvalidOperationException("Bu e-posta veya kullanıcı adı başka bir hesap tarafından kullanılmaktadır.");

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = dto.Email.Trim();
        user.Username = dto.Username.Trim();
        user.PhoneNumber = dto.PhoneNumber?.Trim();
        user.Role = dto.Role;
        user.IsActive = dto.IsActive;

        // Şifre değiştirme isteğe bağlı
        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            _passwordHasher.CreatePasswordHash(dto.NewPassword, out string hash, out string salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
        }

        await _context.SaveChangesAsync();
    }
}
