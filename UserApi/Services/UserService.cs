using UserApi.DTOs;
using UserApi.Models;
using UserApi.Repository;
namespace UserApi.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(u => new UserReadDto{Id = u.Id, Name = u.Name, Email = u.Email});
        }

        public async Task<UserReadDto?> GetUserByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return null;
            return new UserReadDto { Id = user.Id, Name = user.Name, Email = user.Email };
        }

        public async Task<UserReadDto> CreateUserAsync(UserCreateDto dto)
        {
            var user = new User{Name = dto.Name, Email = dto.Email};
            var createdUser = await _repository.CreateAsync(user);
            return new UserReadDto { Id = createdUser.Id, Name = createdUser.Name, Email = createdUser.Email };
        }

        public async Task<UserReadDto> UpdateUserAsync(int id, UserCreateDto dto)
        {
            var user = new User { Name = dto.Name, Email = dto.Email };
            var updatedUser = await _repository.UpdateAsync(id, user);
            if (updatedUser == null) return null;
            return new UserReadDto { Id = updatedUser.Id, Name = updatedUser.Name, Email = updatedUser.Email };
        }
    
        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}

