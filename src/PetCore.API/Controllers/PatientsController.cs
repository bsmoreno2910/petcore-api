using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Common;
using PetCore.API.DTOs.Patients;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/patients")]
[Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Receptionist")]
public class PatientsController : ControllerBase
{
    private readonly PatientService _patientService;
    private readonly IMapper _mapper;

    public PatientsController(PatientService patientService, IMapper mapper)
    {
        _patientService = patientService;
        _mapper = mapper;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? speciesId = null,
        [FromQuery] Guid? tutorId = null)
    {
        var (items, totalCount) = await _patientService.GetPagedAsync(
            GetClinicId(), page, pageSize, search, speciesId, tutorId);

        return Ok(new PagedResponse<PatientDto>
        {
            Items = _mapper.Map<List<PatientDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id, GetClinicId());
        return patient == null ? NotFound() : Ok(_mapper.Map<PatientDetailDto>(patient));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request)
    {
        var patient = new Patient
        {
            TutorId = request.TutorId,
            SpeciesId = request.SpeciesId,
            BreedId = request.BreedId,
            Name = request.Name,
            Sex = Enum.TryParse<PatientSex>(request.Sex, true, out var sex) ? sex : PatientSex.Unknown,
            BirthDate = request.BirthDate,
            Weight = request.Weight,
            Color = request.Color,
            Microchip = request.Microchip,
            Neutered = request.Neutered,
            Allergies = request.Allergies,
            Notes = request.Notes,
            PhotoUrl = request.PhotoUrl
        };

        var created = await _patientService.CreateAsync(patient, GetClinicId());
        var dto = _mapper.Map<PatientDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientRequest request)
    {
        var patient = await _patientService.UpdateAsync(id, GetClinicId(), p =>
        {
            p.TutorId = request.TutorId;
            p.SpeciesId = request.SpeciesId;
            p.BreedId = request.BreedId;
            p.Name = request.Name;
            p.Sex = Enum.TryParse<PatientSex>(request.Sex, true, out var sex) ? sex : PatientSex.Unknown;
            p.BirthDate = request.BirthDate;
            p.Weight = request.Weight;
            p.Color = request.Color;
            p.Microchip = request.Microchip;
            p.Neutered = request.Neutered;
            p.Allergies = request.Allergies;
            p.Notes = request.Notes;
            p.PhotoUrl = request.PhotoUrl;
        });

        return patient == null ? NotFound() : Ok(_mapper.Map<PatientDto>(patient));
    }

    [HttpGet("{id:guid}/medical-records")]
    public async Task<IActionResult> GetMedicalRecords(Guid id)
    {
        var records = await _patientService.GetMedicalRecordsAsync(id, GetClinicId());
        return Ok(records.Select(r => new
        {
            r.Id,
            r.VeterinarianId,
            VeterinarianName = r.Veterinarian.Name,
            r.ChiefComplaint,
            r.Diagnosis,
            r.Treatment,
            r.CreatedAt,
            PrescriptionCount = r.Prescriptions.Count
        }));
    }

    [HttpGet("{id:guid}/exams")]
    public async Task<IActionResult> GetExams(Guid id)
    {
        var exams = await _patientService.GetExamsAsync(id, GetClinicId());
        return Ok(exams.Select(e => new
        {
            e.Id,
            ExamTypeName = e.ExamType.Name,
            RequestedByName = e.RequestedBy.Name,
            Status = e.Status.ToString(),
            e.RequestedAt,
            e.CompletedAt,
            HasResult = e.Result != null
        }));
    }

    [HttpGet("{id:guid}/hospitalizations")]
    public async Task<IActionResult> GetHospitalizations(Guid id)
    {
        var hosps = await _patientService.GetHospitalizationsAsync(id, GetClinicId());
        return Ok(hosps.Select(h => new
        {
            h.Id,
            VeterinarianName = h.Veterinarian.Name,
            Status = h.Status.ToString(),
            h.Reason,
            h.AdmittedAt,
            h.DischargedAt
        }));
    }

    [HttpGet("{id:guid}/timeline")]
    public async Task<IActionResult> GetTimeline(Guid id)
    {
        var clinicId = GetClinicId();

        var records = (await _patientService.GetMedicalRecordsAsync(id, clinicId))
            .Select(r => new { Type = "medical-record", Date = r.CreatedAt, r.Id, Description = r.ChiefComplaint ?? r.Diagnosis ?? "Atendimento" });

        var exams = (await _patientService.GetExamsAsync(id, clinicId))
            .Select(e => new { Type = "exam", Date = e.CreatedAt, e.Id, Description = e.ExamType.Name });

        var hosps = (await _patientService.GetHospitalizationsAsync(id, clinicId))
            .Select(h => new { Type = "hospitalization", Date = h.CreatedAt, h.Id, Description = h.Reason ?? "Internação" });

        var timeline = records
            .Concat(exams)
            .Concat(hosps)
            .OrderByDescending(t => t.Date)
            .ToList();

        return Ok(timeline);
    }
}
