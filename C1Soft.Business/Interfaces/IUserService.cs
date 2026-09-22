using C1Soft.Business.DTOs.User;

namespace C1Soft.Business.Interfaces;

public interface IUserService
{
    Task<List<UserListDto>> GetAllUsersAsync(string? searchQuery);
    Task<UserEditDto?> GetUserForEditAsync(int id);
    Task UpdateUserAsync(UserEditDto dto);
}
