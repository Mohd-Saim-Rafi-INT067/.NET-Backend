using UserApi.Models;
namespace UserApi.Service;

public interface IUserService
{
    IEnumerable<User> GetAllUsers();
    User? GetById(int id); //? -> may return user or may return null
    void CreateUser(User user);
    bool UpdateUser(int id, User user);
    bool DeleteUser(int id);
}