using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Appointments;
using PetCore.API.DTOs.Common;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _appointmentService;
    private readonly IMapper _mapper;

    public AppointmentsController(AppointmentService appointmentService, IMapper mapper)
    {
        _appointmentService = appointmentService;
        _mapper = mapper;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist,Viewer")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? date = null,
        [FromQuery] Guid? veterinarianId = null,
        [FromQuery] AppointmentStatus? status = null,
        [FromQuery] AppointmentType? type = null)
    {
        var (items, totalCount) = await _appointmentService.GetPagedAsync(
            GetClinicId(), page, pageSize, date, veterinarianId, status, type);

        return Ok(new PagedResponse<AppointmentDto>
        {
            Items = _mapper.Map<List<AppointmentDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("calendar")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist,Viewer")]
    public async Task<IActionResult> GetCalendar(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var appointments = await _appointmentService.GetCalendarAsync(GetClinicId(), startDate, endDate);
        var events = _mapper.Map<List<CalendarEventDto>>(appointments);

        // Cores por tipo de agendamento
        foreach (var evt in events)
        {
            evt.Color = evt.Type switch
            {
                "Consultation" => "#3b82f6",
                "Return" => "#22c55e",
                "Surgery" => "#ef4444",
                "Exam" => "#f97316",
                "Vaccination" => "#a855f7",
                "GroomingBath" => "#06b6d4",
                "Emergency" => "#dc2626",
                _ => "#6b7280"
            };
        }

        return Ok(events);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist,Viewer")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id, GetClinicId());
        return appointment == null ? NotFound() : Ok(_mapper.Map<AppointmentDto>(appointment));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        if (!Enum.TryParse<AppointmentType>(request.Type, true, out var type))
            return BadRequest(new { error = "Tipo de agendamento inválido." });

        try
        {
            var appointment = await _appointmentService.CreateAsync(
                GetClinicId(), request.PatientId, request.VeterinarianId,
                type, request.ScheduledAt, request.DurationMinutes,
                request.Reason, request.Notes);

            var dto = _mapper.Map<AppointmentDto>(appointment);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentRequest request)
    {
        var appointment = await _appointmentService.UpdateAsync(id, GetClinicId(), a =>
        {
            if (request.VeterinarianId.HasValue) a.VeterinarianId = request.VeterinarianId;
            if (request.Type != null && Enum.TryParse<AppointmentType>(request.Type, true, out var t)) a.Type = t;
            if (request.ScheduledAt.HasValue) a.ScheduledAt = request.ScheduledAt.Value;
            if (request.DurationMinutes.HasValue) a.DurationMinutes = request.DurationMinutes.Value;
            if (request.Reason != null) a.Reason = request.Reason;
            if (request.Notes != null) a.Notes = request.Notes;
        });

        return appointment == null ? NotFound() : Ok(_mapper.Map<AppointmentDto>(appointment));
    }

    [HttpPatch("{id:guid}/confirm")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public Task<IActionResult> Confirm(Guid id) =>
        ChangeStatus(id, AppointmentStatus.Confirmed);

    [HttpPatch("{id:guid}/check-in")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public Task<IActionResult> CheckIn(Guid id) =>
        ChangeStatus(id, AppointmentStatus.CheckedIn);

    [HttpPatch("{id:guid}/start")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public Task<IActionResult> Start(Guid id) =>
        ChangeStatus(id, AppointmentStatus.InProgress);

    [HttpPatch("{id:guid}/complete")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public Task<IActionResult> Complete(Guid id) =>
        ChangeStatus(id, AppointmentStatus.Completed);

    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelAppointmentRequest? request)
    {
        try
        {
            var appointment = await _appointmentService.ChangeStatusAsync(
                id, GetClinicId(), AppointmentStatus.Cancelled, request?.CancellationReason);
            return appointment == null ? NotFound() : Ok(_mapper.Map<AppointmentDto>(appointment));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/no-show")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public Task<IActionResult> NoShow(Guid id) =>
        ChangeStatus(id, AppointmentStatus.NoShow);

    [HttpGet("available-slots")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] Guid veterinarianId,
        [FromQuery] DateTime date,
        [FromQuery] int slotDuration = 30)
    {
        var slots = await _appointmentService.GetAvailableSlotsAsync(
            GetClinicId(), veterinarianId, date, slotDuration);

        return Ok(slots.Select(s => new AvailableSlotDto { Start = s.Start, End = s.End }));
    }

    private async Task<IActionResult> ChangeStatus(Guid id, AppointmentStatus newStatus)
    {
        try
        {
            var appointment = await _appointmentService.ChangeStatusAsync(id, GetClinicId(), newStatus);
            return appointment == null ? NotFound() : Ok(_mapper.Map<AppointmentDto>(appointment));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
