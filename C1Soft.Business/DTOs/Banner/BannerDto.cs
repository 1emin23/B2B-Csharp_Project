namespace C1Soft.Business.DTOs.Banner;

public class BannerDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? RedirectUrl { get; set; }
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BannerCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? ImageUrl { get; set; }
    public string? RedirectUrl { get; set; }
    public int OrderIndex { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}

public class BannerEditDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? ExistingImageUrl { get; set; }
    public string? NewImageUrl { get; set; }
    public bool RemoveExistingImage { get; set; }
    public string? RedirectUrl { get; set; }
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; }
}
