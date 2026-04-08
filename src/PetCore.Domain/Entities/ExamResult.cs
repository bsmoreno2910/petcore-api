namespace PetCore.Domain.Entities;

public class ExamResult
{
    public Guid Id { get; set; }
    public Guid ExamRequestId { get; set; }
    public Guid PerformedById { get; set; }

    public string? ResultText { get; set; }
    public string? ResultFileUrl { get; set; }
    public string? ReferenceValues { get; set; }
    public string? Observations { get; set; }
    public string? Conclusion { get; set; }

    public DateTime CreatedAt { get; set; }

    public ExamRequest ExamRequest { get; set; } = null!;
    public User PerformedBy { get; set; } = null!;
}
