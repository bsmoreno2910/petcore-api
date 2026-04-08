using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Clinics;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/clinics")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class ClinicsController : ControllerBase
{
    private readonly ClinicService _clinicService;
    private readonly IMapper _mapper;

    public ClinicsController(ClinicService clinicService, IMapper mapper)
    {
        _clinicService = clinicService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clinics = await _clinicService.GetAllAsync();
        return Ok(_mapper.Map<List<ClinicDto>>(clinics));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var clinic = await _clinicService.GetByIdAsync(id);
        return clinic == null ? NotFound() : Ok(_mapper.Map<ClinicDto>(clinic));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateClinicRequest request)
    {
        var clinic = _mapper.Map<Clinic>(request);
        var created = await _clinicService.CreateAsync(clinic);
        var dto = _mapper.Map<ClinicDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClinicRequest request)
    {
        var clinic = await _clinicService.UpdateAsync(id, c => _mapper.Map(request, c));
        return clinic == null ? NotFound() : Ok(_mapper.Map<ClinicDto>(clinic));
    }

    [HttpPatch("{id:guid}/toggle-active")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var result = await _clinicService.ToggleActiveAsync(id);
        return result ? Ok(new { message = "Status alterado." }) : NotFound();
    }

    [HttpGet("{id:guid}/users")]
    public async Task<IActionResult> GetClinicUsers(Guid id)
    {
        var users = await _clinicService.GetClinicUsersAsync(id);
        return Ok(_mapper.Map<List<ClinicUserDto>>(users));
    }

    [HttpPost("{id:guid}/users")]
    public async Task<IActionResult> AddUserToClinic(Guid id, [FromBody] AddClinicUserRequest request)
    {
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            return BadRequest(new { error = "Role inválida." });

        try
        {
            var result = await _clinicService.AddUserToClinicAsync(id, request.UserId, role);
            return CreatedAtAction(nameof(GetClinicUsers), new { id }, _mapper.Map<ClinicUserDto>(result));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/users/{userId:guid}")]
    public async Task<IActionResult> RemoveUserFromClinic(Guid id, Guid userId)
    {
        var result = await _clinicService.RemoveUserFromClinicAsync(id, userId);
        return result ? NoContent() : NotFound();
    }
}
