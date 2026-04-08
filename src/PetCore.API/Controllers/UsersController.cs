using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Users;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IMapper _mapper;

    public UsersController(UserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(_mapper.Map<List<UserDto>>(users));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(_mapper.Map<UserDto>(user));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateAsync(
                request.Name, request.Email, request.Password, request.Phone, request.Crmv);
            var dto = _mapper.Map<UserDto>(user);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userService.UpdateAsync(id, u =>
        {
            u.Name = request.Name;
            if (request.Phone != null) u.Phone = request.Phone;
            if (request.Crmv != null) u.Crmv = request.Crmv;
            if (request.AvatarUrl != null) u.AvatarUrl = request.AvatarUrl;
        });
        return user == null ? NotFound() : Ok(_mapper.Map<UserDto>(user));
    }

    [HttpPatch("{id:guid}/toggle-active")]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var result = await _userService.ToggleActiveAsync(id);
        return result ? Ok(new { message = "Status alterado." }) : NotFound();
    }
}
