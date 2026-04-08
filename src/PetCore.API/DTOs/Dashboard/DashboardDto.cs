namespace PetCore.API.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalPatients { get; set; }
    public int TotalTutors { get; set; }
    public int AppointmentsToday { get; set; }
    public int ActiveHospitalizations { get; set; }
    public int PendingExams { get; set; }
    public int LowStockProducts { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal MonthlyExpense { get; set; }
}

public class DashboardAppointmentDto
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string TutorName { get; set; } = string.Empty;
    public string? VeterinarianName { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
}

public class DashboardFinancialDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
    public List<DailyFinancialDto> Daily { get; set; } = [];
}

public class DailyFinancialDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expense { get; set; }
}

public class DashboardStockAlertDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinStock { get; set; }
    public string AlertType { get; set; } = string.Empty; // Zero, Low, Expiring
    public DateTime? ExpirationDate { get; set; }
}

public class TopServiceDto
{
    public string ServiceName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class PatientsChartDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}
