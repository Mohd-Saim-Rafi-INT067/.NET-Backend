using Microsoft.AspNetCore.Mvc;
using UserApi.Models;
using UserApi.Service;

namespace UserApi.Controllers;

[Route("api/users")]
[ApiController]
public class UserController: ControllerBase
{
    private readonly IUserService _service;
    public UserController(IUserService service)
    {
        _service = service;
    }
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        return Ok(_service.GetAllUsers());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _service.GetById(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        _service.Create(user);
        return CreatedAtAction(nameof(GetById), new {id = user.Id}, user);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, User user)
    {
        var success = _service.Update(id, user);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var success = _service.Delete(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

}