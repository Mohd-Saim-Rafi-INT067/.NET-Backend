using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}