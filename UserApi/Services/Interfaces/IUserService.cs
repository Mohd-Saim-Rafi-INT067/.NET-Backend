using UserApi.DTOs;

namespace UserApi.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
        Task<UserReadDto?> GetUserByIdAsync(int id);
        Task<UserReadDto> CreateUserAsync(UserCreateDto user);
        Task<UserReadDto?> UpdateUserAsync(int id, UserCreateDto user);
        Task<bool> DeleteUserAsync(int id);
    }
    
}
