using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Common;
using PetCore.API.DTOs.MedicalRecords;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/medical-records")]
[Authorize]
public class MedicalRecordsController : ControllerBase
{
    private readonly MedicalRecordService _service;
    private readonly IMapper _mapper;

    public MedicalRecordsController(MedicalRecordService service, IMapper mapper)
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
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? veterinarianId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (items, totalCount) = await _service.GetPagedAsync(
            GetClinicId(), page, pageSize, patientId, veterinarianId, startDate, endDate);

        return Ok(new PagedResponse<MedicalRecordDto>
        {
            Items = _mapper.Map<List<MedicalRecordDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var record = await _service.GetByIdAsync(id, GetClinicId());
        return record == null ? NotFound() : Ok(_mapper.Map<MedicalRecordDto>(record));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian")]
    public async Task<IActionResult> Create([FromBody] CreateMedicalRecordRequest request)
    {
        var record = new MedicalRecord
        {
            PatientId = request.PatientId,
            AppointmentId = request.AppointmentId,
            ChiefComplaint = request.ChiefComplaint,
            History = request.History,
            Anamnesis = request.Anamnesis,
            Weight = request.Weight,
            Temperature = request.Temperature,
            HeartRate = request.HeartRate,
            RespiratoryRate = request.RespiratoryRate,
            PhysicalExam = request.PhysicalExam,
            Mucous = request.Mucous,
            Hydration = request.Hydration,
            Lymph = request.Lymph,
            Diagnosis = request.Diagnosis,
            DifferentialDiagnosis = request.DifferentialDiagnosis,
            Treatment = request.Treatment,
            Notes = request.Notes,
            InternalNotes = request.InternalNotes
        };

        var created = await _service.CreateAsync(record, GetClinicId(), GetUserId());
        var dto = _mapper.Map<MedicalRecordDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicalRecordRequest request)
    {
        var record = await _service.UpdateAsync(id, GetClinicId(), m =>
        {
            m.ChiefComplaint = request.ChiefComplaint;
            m.History = request.History;
            m.Anamnesis = request.Anamnesis;
            m.Weight = request.Weight;
            m.Temperature = request.Temperature;
            m.HeartRate = request.HeartRate;
            m.RespiratoryRate = request.RespiratoryRate;
            m.PhysicalExam = request.PhysicalExam;
            m.Mucous = request.Mucous;
            m.Hydration = request.Hydration;
            m.Lymph = request.Lymph;
            m.Diagnosis = request.Diagnosis;
            m.DifferentialDiagnosis = request.DifferentialDiagnosis;
            m.Treatment = request.Treatment;
            m.Notes = request.Notes;
            m.InternalNotes = request.InternalNotes;
        });

        return record == null ? NotFound() : Ok(_mapper.Map<MedicalRecordDto>(record));
    }

    [HttpPost("{id:guid}/prescriptions")]
    [Authorize(Roles = "SuperAdmin,Veterinarian")]
    public async Task<IActionResult> AddPrescription(Guid id, [FromBody] CreatePrescriptionRequest request)
    {
        try
        {
            var prescription = new Prescription
            {
                MedicineName = request.MedicineName,
                Dosage = request.Dosage,
                Frequency = request.Frequency,
                Duration = request.Duration,
                Route = request.Route,
                Instructions = request.Instructions,
                Quantity = request.Quantity
            };

            var created = await _service.AddPrescriptionAsync(id, GetClinicId(), prescription);
            return Created($"/api/medical-records/{id}/prescriptions", _mapper.Map<PrescriptionDto>(created));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/prescriptions/{prescId:guid}")]
    [Authorize(Roles = "SuperAdmin,Veterinarian")]
    public async Task<IActionResult> RemovePrescription(Guid id, Guid prescId)
    {
        var result = await _service.RemovePrescriptionAsync(id, prescId, GetClinicId());
        return result ? NoContent() : NotFound();
    }
}
