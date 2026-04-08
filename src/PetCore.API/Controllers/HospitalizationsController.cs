using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Common;
using PetCore.API.DTOs.Hospitalizations;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/hospitalizations")]
[Authorize]
public class HospitalizationsController : ControllerBase
{
    private readonly HospitalizationService _service;
    private readonly IMapper _mapper;

    public HospitalizationsController(HospitalizationService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;
    private Guid GetUserId() => (Guid)HttpContext.Items["UserId"]!;

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] HospitalizationStatus? status = null,
        [FromQuery] Guid? patientId = null)
    {
        var (items, totalCount) = await _service.GetPagedAsync(GetClinicId(), page, pageSize, status, patientId);

        return Ok(new PagedResponse<HospitalizationDto>
        {
            Items = _mapper.Map<List<HospitalizationDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var hosp = await _service.GetByIdAsync(id, GetClinicId());
        return hosp == null ? NotFound() : Ok(_mapper.Map<HospitalizationDetailDto>(hosp));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian")]
    public async Task<IActionResult> Create([FromBody] CreateHospitalizationRequest request)
    {
        var created = await _service.CreateAsync(
            GetClinicId(), GetUserId(), request.PatientId,
            request.Reason, request.Cage, request.Diet, request.Notes, request.AdmittedAt);

        var dto = _mapper.Map<HospitalizationDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHospitalizationRequest request)
    {
        var hosp = await _service.UpdateAsync(id, GetClinicId(), h =>
        {
            if (request.Cage != null) h.Cage = request.Cage;
            if (request.Diet != null) h.Diet = request.Diet;
            if (request.Notes != null) h.Notes = request.Notes;
        });

        return hosp == null ? NotFound() : Ok(_mapper.Map<HospitalizationDto>(hosp));
    }

    [HttpPatch("{id:guid}/discharge")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian")]
    public async Task<IActionResult> Discharge(Guid id, [FromBody] DischargeRequest? request)
    {
        try
        {
            var hosp = await _service.DischargeAsync(id, GetClinicId(), request?.DischargeNotes);
            return hosp == null ? NotFound() : Ok(_mapper.Map<HospitalizationDto>(hosp));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/evolutions")]
    [Authorize(Roles = "SuperAdmin,Veterinarian")]
    public async Task<IActionResult> AddEvolution(Guid id, [FromBody] CreateEvolutionRequest request)
    {
        try
        {
            var evolution = new HospitalizationEvolution
            {
                Weight = request.Weight,
                Temperature = request.Temperature,
                HeartRate = request.HeartRate,
                RespiratoryRate = request.RespiratoryRate,
                Description = request.Description,
                Medications = request.Medications,
                Feeding = request.Feeding,
                Notes = request.Notes
            };

            var created = await _service.AddEvolutionAsync(id, GetClinicId(), GetUserId(), evolution);
            return Created($"/api/hospitalizations/{id}/evolutions", _mapper.Map<EvolutionDto>(created));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}/evolutions")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> GetEvolutions(Guid id)
    {
        var evolutions = await _service.GetEvolutionsAsync(id, GetClinicId());
        return Ok(_mapper.Map<List<EvolutionDto>>(evolutions));
    }
}
