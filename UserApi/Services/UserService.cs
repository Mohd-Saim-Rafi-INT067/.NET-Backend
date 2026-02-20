using UserApi.Models;
namespace UserApi.Service;

public class UserService : IUserService
{
    private static readonly Dictionary<int, User> _users = new();
    private static int _nextId = 1;

    public IEnumerable<User> GetAllUsers()
    {
        return _users.Values;
    }

    public User? GetById(int id)
    {
        return _users.ContainsKey(id) ? _users[id] : null;
    }

    public void Create(User user)
    {
        user.Id = _nextId++;
        _users[user.Id] = user;
    }

    public bool Update(int id, User user)
    {
        if (!_users.ContainsKey(id))
        {
            return false;
        }
        user.Id = id;
        _users[id] = user;
        return true;
    }

    public bool Delete(int id)
    {
        return _users.Remove(id);
    }
}

