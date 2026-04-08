using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Common;
using PetCore.API.DTOs.Patients;
using PetCore.API.DTOs.Tutors;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/tutors")]
[Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
public class TutorsController : ControllerBase
{
    private readonly TutorService _tutorService;
    private readonly IMapper _mapper;

    public TutorsController(TutorService tutorService, IMapper mapper)
    {
        _tutorService = tutorService;
        _mapper = mapper;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? phone = null,
        [FromQuery] string? cpf = null)
    {
        var (items, totalCount) = await _tutorService.GetPagedAsync(GetClinicId(), page, pageSize, search, phone, cpf);
        return Ok(new PagedResponse<TutorDto>
        {
            Items = _mapper.Map<List<TutorDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tutor = await _tutorService.GetByIdAsync(id, GetClinicId());
        return tutor == null ? NotFound() : Ok(_mapper.Map<TutorDetailDto>(tutor));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTutorRequest request)
    {
        var tutor = _mapper.Map<Tutor>(request);
        var created = await _tutorService.CreateAsync(tutor, GetClinicId());
        var dto = _mapper.Map<TutorDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTutorRequest request)
    {
        var tutor = await _tutorService.UpdateAsync(id, GetClinicId(), t => _mapper.Map(request, t));
        return tutor == null ? NotFound() : Ok(_mapper.Map<TutorDto>(tutor));
    }

    [HttpGet("{id:guid}/patients")]
    public async Task<IActionResult> GetPatients(Guid id)
    {
        var patients = await _tutorService.GetPatientsAsync(id, GetClinicId());
        return Ok(_mapper.Map<List<PatientDto>>(patients));
    }

    [HttpGet("{id:guid}/financial-summary")]
    public async Task<IActionResult> GetFinancialSummary(Guid id)
    {
        var (totalRevenue, totalPaid, totalPending, totalOverdue) =
            await _tutorService.GetFinancialSummaryAsync(id, GetClinicId());

        return Ok(new TutorFinancialSummaryDto
        {
            TotalRevenue = totalRevenue,
            TotalPaid = totalPaid,
            TotalPending = totalPending,
            TotalOverdue = totalOverdue
        });
    }
}
