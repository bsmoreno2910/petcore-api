using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class AppointmentService
{
    private readonly AppDbContext _db;

    public AppointmentService(AppDbContext db)
    {
        _db = db;
    }

    private IQueryable<Appointment> BaseQuery(Guid clinicId) =>
        _db.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.Tutor)
            .Include(a => a.Patient).ThenInclude(p => p.Species)
            .Include(a => a.Veterinarian)
            .Where(a => a.ClinicId == clinicId);

    public async Task<(List<Appointment> Items, int TotalCount)> GetPagedAsync(
        Guid clinicId, int page, int pageSize,
        DateTime? date, Guid? veterinarianId, AppointmentStatus? status, AppointmentType? type)
    {
        var query = BaseQuery(clinicId);

        if (date.HasValue)
        {
            var start = date.Value.Date;
            var end = start.AddDays(1);
            query = query.Where(a => a.ScheduledAt >= start && a.ScheduledAt < end);
        }

        if (veterinarianId.HasValue)
            query = query.Where(a => a.VeterinarianId == veterinarianId.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(a => a.ScheduledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<Appointment>> GetCalendarAsync(Guid clinicId, DateTime startDate, DateTime endDate)
    {
        return await BaseQuery(clinicId)
            .Where(a => a.ScheduledAt >= startDate && a.ScheduledAt <= endDate)
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(Guid id, Guid clinicId)
    {
        return await BaseQuery(clinicId).FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Appointment> CreateAsync(
        Guid clinicId, Guid patientId, Guid? veterinarianId,
        AppointmentType type, DateTime scheduledAt, int durationMinutes,
        string? reason, string? notes)
    {
        // Verificar conflito de horário para o veterinário
        if (veterinarianId.HasValue)
        {
            var endTime = scheduledAt.AddMinutes(durationMinutes);
            var conflict = await _db.Appointments
                .AnyAsync(a =>
                    a.ClinicId == clinicId &&
                    a.VeterinarianId == veterinarianId.Value &&
                    a.Status != AppointmentStatus.Cancelled &&
                    a.Status != AppointmentStatus.NoShow &&
                    a.ScheduledAt < endTime &&
                    a.ScheduledAt.AddMinutes(a.DurationMinutes) > scheduledAt);

            if (conflict)
                throw new InvalidOperationException("Já existe um agendamento neste horário para este veterinário.");
        }

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            ClinicId = clinicId,
            PatientId = patientId,
            VeterinarianId = veterinarianId,
            Type = type,
            Status = AppointmentStatus.Scheduled,
            ScheduledAt = scheduledAt,
            DurationMinutes = durationMinutes,
            Reason = reason,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(appointment.Id, clinicId))!;
    }

    public async Task<Appointment?> UpdateAsync(Guid id, Guid clinicId, Action<Appointment> updateAction)
    {
        var appointment = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.ClinicId == clinicId);
        if (appointment == null) return null;

        updateAction(appointment);
        appointment.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await GetByIdAsync(id, clinicId);
    }

    public async Task<Appointment?> ChangeStatusAsync(Guid id, Guid clinicId, AppointmentStatus newStatus, string? cancellationReason = null)
    {
        var appointment = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.ClinicId == clinicId);
        if (appointment == null) return null;

        // Validar transição de status
        var validTransitions = GetValidTransitions(appointment.Status);
        if (!validTransitions.Contains(newStatus))
            throw new InvalidOperationException(
                $"Não é possível alterar o status de '{appointment.Status}' para '{newStatus}'.");

        appointment.Status = newStatus;
        appointment.UpdatedAt = DateTime.UtcNow;

        switch (newStatus)
        {
            case AppointmentStatus.InProgress:
                appointment.StartedAt = DateTime.UtcNow;
                break;
            case AppointmentStatus.Completed:
                appointment.FinishedAt = DateTime.UtcNow;
                break;
            case AppointmentStatus.Cancelled:
                appointment.CancellationReason = cancellationReason;
                break;
        }

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, clinicId);
    }

    public async Task<List<AvailableSlot>> GetAvailableSlotsAsync(
        Guid clinicId, Guid veterinarianId, DateTime date, int slotDuration = 30)
    {
        var startOfDay = date.Date.AddHours(8);  // 08:00
        var endOfDay = date.Date.AddHours(18);    // 18:00

        var existingAppointments = await _db.Appointments
            .Where(a =>
                a.ClinicId == clinicId &&
                a.VeterinarianId == veterinarianId &&
                a.ScheduledAt >= startOfDay &&
                a.ScheduledAt < endOfDay &&
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.NoShow)
            .OrderBy(a => a.ScheduledAt)
            .Select(a => new { a.ScheduledAt, a.DurationMinutes })
            .ToListAsync();

        var slots = new List<AvailableSlot>();
        var current = startOfDay;

        while (current.AddMinutes(slotDuration) <= endOfDay)
        {
            var slotEnd = current.AddMinutes(slotDuration);
            var hasConflict = existingAppointments.Any(a =>
                a.ScheduledAt < slotEnd && a.ScheduledAt.AddMinutes(a.DurationMinutes) > current);

            if (!hasConflict)
            {
                slots.Add(new AvailableSlot { Start = current, End = slotEnd });
            }

            current = current.AddMinutes(slotDuration);
        }

        return slots;
    }

    private static HashSet<AppointmentStatus> GetValidTransitions(AppointmentStatus current) => current switch
    {
        AppointmentStatus.Scheduled => [AppointmentStatus.Confirmed, AppointmentStatus.Cancelled, AppointmentStatus.NoShow],
        AppointmentStatus.Confirmed => [AppointmentStatus.CheckedIn, AppointmentStatus.Cancelled, AppointmentStatus.NoShow],
        AppointmentStatus.CheckedIn => [AppointmentStatus.InProgress, AppointmentStatus.Cancelled],
        AppointmentStatus.InProgress => [AppointmentStatus.Completed],
        _ => []
    };
}

public class AvailableSlot
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
