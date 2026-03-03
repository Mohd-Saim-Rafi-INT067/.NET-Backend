using Microsoft.AspNetCore.Mvc;
using UserApi.DTOs;
using UserApi.Service;
using UserApi.Responses;

namespace UserApi.Controllers{
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
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _UserService.GetAllUsersAsync();
            return Ok(new ApiResponse(200, true, "Users retrieved successfully", data: users));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _UserService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse(404, false, $"User with id {id} not found."));
            }
            return Ok(new ApiResponse(200, true, "User retrieved successfully", data: user));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            var user = await _UserService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new {id = user.Id}, new ApiResponse(201, true, "User created successfully", data: user));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserCreateDto dto)
        {
            var updatedUser = await _UserService.UpdateUserAsync(id, dto);
            if (updatedUser == null)
            {
                return NotFound(new ApiResponse(404, false, $"User with id {id} not found."));
            }
            return Ok(new ApiResponse(200, true, "User updated successfully", data: updatedUser));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _UserService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponse(404, false, $"User with id {id} not found."));
            }
            return NoContent();
        }

    }

}