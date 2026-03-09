using Microsoft.EntityFrameworkCore;
using Login_Register.Models;

namespace Login_Register.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options){
    public DbSet<User> Users => Set<User>();
}