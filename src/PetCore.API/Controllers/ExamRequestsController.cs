using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Common;
using PetCore.API.DTOs.Exams;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/exam-requests")]
[Authorize]
public class ExamRequestsController : ControllerBase
{
    private readonly ExamService _examService;
    private readonly IMapper _mapper;

    public ExamRequestsController(ExamService examService, IMapper mapper)
    {
        _examService = examService;
        _mapper = mapper;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;
    private Guid GetUserId() => (Guid)HttpContext.Items["UserId"]!;

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] ExamStatus? status = null,
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? examTypeId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (items, totalCount) = await _examService.GetExamRequestsPagedAsync(
            GetClinicId(), page, pageSize, status, patientId, examTypeId, startDate, endDate);

        return Ok(new PagedResponse<ExamRequestDto>
        {
            Items = _mapper.Map<List<ExamRequestDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var exam = await _examService.GetExamRequestByIdAsync(id, GetClinicId());
        return exam == null ? NotFound() : Ok(_mapper.Map<ExamRequestDto>(exam));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian")]
    public async Task<IActionResult> Create([FromBody] CreateExamRequestDto request)
    {
        var exam = await _examService.CreateExamRequestAsync(
            GetClinicId(), GetUserId(), request.PatientId, request.ExamTypeId,
            request.MedicalRecordId, request.ClinicalIndication, request.Notes);

        var dto = _mapper.Map<ExamRequestDto>(exam);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPatch("{id:guid}/collect")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian")]
    public async Task<IActionResult> Collect(Guid id)
    {
        try
        {
            var exam = await _examService.CollectAsync(id, GetClinicId());
            return exam == null ? NotFound() : Ok(_mapper.Map<ExamRequestDto>(exam));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var exam = await _examService.CancelAsync(id, GetClinicId());
            return exam == null ? NotFound() : Ok(_mapper.Map<ExamRequestDto>(exam));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/result")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian")]
    public async Task<IActionResult> AddResult(Guid id, [FromBody] CreateExamResultRequest request)
    {
        try
        {
            var result = await _examService.AddResultAsync(
                id, GetClinicId(), GetUserId(),
                request.ResultText, request.ResultFileUrl, request.ReferenceValues,
                request.Observations, request.Conclusion);

            return Created($"/api/exam-requests/{id}/result", _mapper.Map<ExamResultDto>(result));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}/result")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> GetResult(Guid id)
    {
        var result = await _examService.GetResultAsync(id, GetClinicId());
        return result == null ? NotFound() : Ok(_mapper.Map<ExamResultDto>(result));
    }
}
