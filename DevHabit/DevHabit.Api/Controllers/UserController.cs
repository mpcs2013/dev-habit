using System.Security.Claims;
using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Users;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

// Allow both Members and Admins to access user endpoints
// [Authorize(Roles = $"{Roles.Member},{Roles.Admin}")]
[Authorize(Roles = Roles.Member)]
[ApiController]
[Route("users")]
public class UserController(ApplicationDbContext dbContext, UserContext userContext) : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetUserById(string id)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (id != userId)
        {
            return Forbid();
        }

        UserDto? user = await dbContext.Users
            .Where(u => u.Id == id)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }


    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        string? userId = await userContext.GetUserIdAsync(); 

        if(string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        UserDto? user = await dbContext.Users
            .Where(u => u.Id == userId)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
