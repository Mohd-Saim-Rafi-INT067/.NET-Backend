using Microsoft.EntityFrameworkCore;
using StudentAPI.Entities;
namespace StudentAPI.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        {
            
        }
        public DbSet<Student> Students { get; set; }
    }
}

