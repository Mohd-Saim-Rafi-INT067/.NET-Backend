using UserApi.Models;
namespace UserApi.Service;

public interface IUserService
{
    IEnumerable<User> GetAllUsers();
    User? GetById(int id); //? -> may return usr or may return null
    void Create(User user);
    bool Update(int id, User user);
    bool Delete(int id);
}