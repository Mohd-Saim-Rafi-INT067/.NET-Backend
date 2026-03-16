// Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using AuthenticationAuthorization.Data;
using AuthenticationAuthorization.Interfaces;
using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(int id)
        => db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByEmailAsync(string email)
        => db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<bool> EmailExistsAsync(string email)
        => db.Users.AnyAsync(u => u.Email == email);

    public Task<bool> UsernameExistsAsync(string username)
        => db.Users.AnyAsync(u => u.Username == username);

    public async Task AddAsync(User user)
        => await db.Users.AddAsync(user);

    public Task SaveChangesAsync()
        => db.SaveChangesAsync();
}