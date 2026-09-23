using C1Soft.Business.DTOs.Banner;

namespace C1Soft.Business.Interfaces;

public interface IBannerService
{
    Task<List<BannerDto>> GetAllBannersAsync();
    Task<BannerEditDto?> GetBannerForEditAsync(int id);
    Task<int> CreateBannerAsync(BannerCreateDto dto);
    Task UpdateBannerAsync(BannerEditDto dto);
    Task<string?> DeleteBannerAsync(int id);
    Task ToggleBannerStatusAsync(int id);
}
