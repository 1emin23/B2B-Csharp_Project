using C1Soft.Business.DTOs.Banner;
using C1Soft.Business.Interfaces;
using C1Soft.DataAccess.Context;
using C1Soft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace C1Soft.Business.Services;

public class BannerService : IBannerService
{
    private readonly AppDbContext _context;

    public BannerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BannerDto>> GetAllBannersAsync()
    {
        return await _context.Banners
            .OrderBy(b => b.OrderIndex)
            .ThenByDescending(b => b.CreatedAt)
            .Select(b => new BannerDto
            {
                Id = b.Id,
                Title = b.Title,
                Subtitle = b.Subtitle,
                ImageUrl = b.ImageUrl,
                RedirectUrl = b.RedirectUrl,
                OrderIndex = b.OrderIndex,
                IsActive = b.IsActive,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<BannerEditDto?> GetBannerForEditAsync(int id)
    {
        return await _context.Banners
            .Where(b => b.Id == id)
            .Select(b => new BannerEditDto
            {
                Id = b.Id,
                Title = b.Title,
                Subtitle = b.Subtitle,
                ExistingImageUrl = b.ImageUrl,
                RedirectUrl = b.RedirectUrl,
                OrderIndex = b.OrderIndex,
                IsActive = b.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateBannerAsync(BannerCreateDto dto)
    {
        var banner = new Banner
        {
            Title = dto.Title.Trim(),
            Subtitle = dto.Subtitle?.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? "/images/banners/default.jpg" : dto.ImageUrl.Trim(),
            RedirectUrl = string.IsNullOrWhiteSpace(dto.RedirectUrl) ? "/Product/Index" : dto.RedirectUrl.Trim(),
            OrderIndex = dto.OrderIndex,
            IsActive = dto.IsActive
        };

        _context.Banners.Add(banner);
        await _context.SaveChangesAsync();
        return banner.Id;
    }

    public async Task UpdateBannerAsync(BannerEditDto dto)
    {
        var banner = await _context.Banners.FindAsync(dto.Id)
            ?? throw new InvalidOperationException($"Banner bulunamadı. Id: {dto.Id}");

        banner.Title = dto.Title.Trim();
        banner.Subtitle = dto.Subtitle?.Trim();
        banner.RedirectUrl = string.IsNullOrWhiteSpace(dto.RedirectUrl) ? "/Product/Index" : dto.RedirectUrl.Trim();
        banner.OrderIndex = dto.OrderIndex;
        banner.IsActive = dto.IsActive;

        if (!string.IsNullOrWhiteSpace(dto.NewImageUrl))
        {
            banner.ImageUrl = dto.NewImageUrl.Trim();
        }

        await _context.SaveChangesAsync();
    }

    public async Task<string?> DeleteBannerAsync(int id)
    {
        var banner = await _context.Banners.FindAsync(id)
            ?? throw new InvalidOperationException($"Banner bulunamadı. Id: {id}");

        var imageUrl = banner.ImageUrl;
        _context.Banners.Remove(banner);
        await _context.SaveChangesAsync();
        return imageUrl;
    }

    public async Task ToggleBannerStatusAsync(int id)
    {
        var banner = await _context.Banners.FindAsync(id)
            ?? throw new InvalidOperationException($"Banner bulunamadı. Id: {id}");

        banner.IsActive = !banner.IsActive;
        await _context.SaveChangesAsync();
    }
}
