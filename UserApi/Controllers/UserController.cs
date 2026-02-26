using Microsoft.AspNetCore.Mvc;
using UserApi.Models;
using UserApi.Service;

namespace UserApi.Controllers;

[Route("api/users")]
[ApiController]
public class UserController: ControllerBase
{
    private readonly IUserService _UserService;
    public UserController(IUserService service)
    {
        _UserService = service;
    }
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _UserService.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _UserService.GetById(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    public IActionResult CreateUser(User user)
    {
        _UserService.CreateUser(user);
        return CreatedAtAction(nameof(GetById), new {id = user.Id}, user);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, User user)
    {
        var success = _UserService.UpdateUser(id, user);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var success = _UserService.DeleteUser(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

}