using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ExcelExportService
{
    private readonly AppDbContext _db;
    private const string HeaderColor = "#1a365d";
    private const string AltRowColor = "#f7fafc";
    private const string TotalRowColor = "#e2e8f0";

    public ExcelExportService(AppDbContext db)
    {
        _db = db;
    }

    // ===== HELPERS =====
    private static void StyleHeader(IXLWorksheet ws, int lastCol, int headerRow = 3)
    {
        var range = ws.Range(headerRow, 1, headerRow, lastCol);
        range.Style.Font.Bold = true;
        range.Style.Font.FontSize = 11;
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(HeaderColor);
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    }

    private static void StyleData(IXLWorksheet ws, int startRow, int endRow, int lastCol)
    {
        for (int r = startRow; r <= endRow; r++)
        {
            if (r % 2 == 0)
                ws.Range(r, 1, r, lastCol).Style.Fill.BackgroundColor = XLColor.FromHtml(AltRowColor);
        }
        ws.Range(startRow, 1, endRow, lastCol).Style.Font.FontSize = 10;
    }

    private static void AddTitle(IXLWorksheet ws, string title, string clinicName, string? period = null)
    {
        ws.Cell(1, 1).Value = clinicName;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Cell(2, 1).Value = title + (period != null ? $" — {period}" : "");
        ws.Cell(2, 1).Style.Font.Bold = true;
        ws.Cell(2, 1).Style.Font.FontSize = 12;
    }

    private static void AddFooter(IXLWorksheet ws, int row, string userName, string clinicName)
    {
        ws.Cell(row, 1).Value = $"PetCore — Gerado em {DateTime.Now:dd/MM/yyyy HH:mm} por {userName} — {clinicName}";
        ws.Cell(row, 1).Style.Font.Italic = true;
        ws.Cell(row, 1).Style.Font.FontSize = 8;
    }

    private static void AutoFitAndFilter(IXLWorksheet ws, int headerRow, int lastCol)
    {
        ws.Columns(1, lastCol).AdjustToContents();
        ws.Range(headerRow, 1, headerRow, lastCol).SetAutoFilter();
    }

    private static byte[] WorkbookToBytes(XLWorkbook wb)
    {
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    // ===== 1. INVENTÁRIO ATUAL =====
    public async Task<byte[]> ExportInventoryAsync(Guid clinicId, string clinicName, string userName,
        Guid? categoryId = null, string? stockStatus = null)
    {
        var query = _db.Products.Include(p => p.Category).Include(p => p.Unit)
            .Where(p => p.ClinicId == clinicId && p.Active);
        if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);
        if (stockStatus == "zero") query = query.Where(p => p.CurrentStock <= 0);
        else if (stockStatus == "low") query = query.Where(p => p.CurrentStock > 0 && p.CurrentStock <= p.MinStock);

        var products = await query.OrderBy(p => p.Category.Name).ThenBy(p => p.Name).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Inventário");
        AddTitle(ws, "Inventário Atual", clinicName);

        var headers = new[] { "Produto", "Categoria", "Unidade", "Estoque Atual", "Estoque Mín.", "Preço Custo", "Preço Venda", "Lote", "Validade", "Status" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(3, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length);

        for (int i = 0; i < products.Count; i++)
        {
            var p = products[i];
            var r = i + 4;
            ws.Cell(r, 1).Value = p.Name;
            ws.Cell(r, 2).Value = p.Category.Name;
            ws.Cell(r, 3).Value = p.Unit.Abbreviation;
            ws.Cell(r, 4).Value = p.CurrentStock;
            ws.Cell(r, 5).Value = p.MinStock;
            ws.Cell(r, 6).Value = p.CostPrice ?? 0;
            ws.Cell(r, 6).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 7).Value = p.SellingPrice ?? 0;
            ws.Cell(r, 7).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 8).Value = p.Batch ?? "";
            ws.Cell(r, 9).Value = p.ExpirationDate?.ToString("dd/MM/yyyy") ?? "";
            var status = p.CurrentStock <= 0 ? "Zerado" : p.CurrentStock <= p.MinStock ? "Baixo" : "Ok";
            ws.Cell(r, 10).Value = status;
            if (status == "Zerado") ws.Cell(r, 10).Style.Font.FontColor = XLColor.Red;
            else if (status == "Baixo") ws.Cell(r, 10).Style.Font.FontColor = XLColor.Orange;
            else ws.Cell(r, 10).Style.Font.FontColor = XLColor.Green;
        }

        StyleData(ws, 4, 3 + products.Count, headers.Length);
        AutoFitAndFilter(ws, 3, headers.Length);
        AddFooter(ws, 5 + products.Count, userName, clinicName);

        // Aba Resumo por Categoria
        var ws2 = wb.Worksheets.Add("Resumo por Categoria");
        AddTitle(ws2, "Resumo por Categoria", clinicName);
        ws2.Cell(3, 1).Value = "Categoria"; ws2.Cell(3, 2).Value = "Qtd. Produtos"; ws2.Cell(3, 3).Value = "Estoque Total"; ws2.Cell(3, 4).Value = "Valor Estoque";
        StyleHeader(ws2, 4);

        var groups = products.GroupBy(p => p.Category.Name).OrderBy(g => g.Key).ToList();
        for (int i = 0; i < groups.Count; i++)
        {
            var g = groups[i]; var r = i + 4;
            ws2.Cell(r, 1).Value = g.Key;
            ws2.Cell(r, 2).Value = g.Count();
            ws2.Cell(r, 3).Value = g.Sum(p => p.CurrentStock);
            ws2.Cell(r, 4).Value = g.Sum(p => p.CurrentStock * (p.CostPrice ?? 0));
            ws2.Cell(r, 4).Style.NumberFormat.Format = "R$ #,##0.00";
        }
        AutoFitAndFilter(ws2, 3, 4);

        return WorkbookToBytes(wb);
    }

    // ===== 2. MOVIMENTAÇÕES DE ESTOQUE =====
    public async Task<byte[]> ExportStockMovementsAsync(Guid clinicId, string clinicName, string userName,
        DateTime? startDate = null, DateTime? endDate = null, MovementType? type = null, Guid? productId = null)
    {
        var query = _db.Movements.Include(m => m.Product).Include(m => m.CreatedBy)
            .Where(m => m.ClinicId == clinicId);
        if (startDate.HasValue) query = query.Where(m => m.CreatedAt >= startDate.Value);
        if (endDate.HasValue) query = query.Where(m => m.CreatedAt <= endDate.Value);
        if (type.HasValue) query = query.Where(m => m.Type == type.Value);
        if (productId.HasValue) query = query.Where(m => m.ProductId == productId.Value);

        var movements = await query.OrderByDescending(m => m.CreatedAt).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Movimentações");
        AddTitle(ws, "Movimentações de Estoque", clinicName);

        var headers = new[] { "Data", "Produto", "Tipo", "Quantidade", "Estoque Anterior", "Novo Estoque", "Motivo", "Usuário" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(3, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length);

        for (int i = 0; i < movements.Count; i++)
        {
            var m = movements[i]; var r = i + 4;
            ws.Cell(r, 1).Value = m.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            ws.Cell(r, 2).Value = m.Product.Name;
            ws.Cell(r, 3).Value = m.Type.ToString();
            ws.Cell(r, 4).Value = m.Quantity;
            ws.Cell(r, 5).Value = m.PreviousStock;
            ws.Cell(r, 6).Value = m.NewStock;
            ws.Cell(r, 7).Value = m.Reason ?? "";
            ws.Cell(r, 8).Value = m.CreatedBy.Name;
        }

        StyleData(ws, 4, 3 + movements.Count, headers.Length);
        AutoFitAndFilter(ws, 3, headers.Length);
        AddFooter(ws, 5 + movements.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 7. ATENDIMENTOS =====
    public async Task<byte[]> ExportAppointmentsAsync(Guid clinicId, string clinicName, string userName,
        DateTime? startDate = null, DateTime? endDate = null, AppointmentType? type = null, Guid? veterinarianId = null)
    {
        var query = _db.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.Tutor)
            .Include(a => a.Veterinarian)
            .Where(a => a.ClinicId == clinicId && a.Status == AppointmentStatus.Completed);
        if (startDate.HasValue) query = query.Where(a => a.FinishedAt >= startDate.Value);
        if (endDate.HasValue) query = query.Where(a => a.FinishedAt <= endDate.Value);
        if (type.HasValue) query = query.Where(a => a.Type == type.Value);
        if (veterinarianId.HasValue) query = query.Where(a => a.VeterinarianId == veterinarianId.Value);

        var appointments = await query.OrderByDescending(a => a.ScheduledAt).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Atendimentos");
        AddTitle(ws, "Relatório de Atendimentos", clinicName);

        var headers = new[] { "Data", "Paciente", "Tutor", "Veterinário", "Tipo", "Duração (min)", "Motivo" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(3, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length);

        for (int i = 0; i < appointments.Count; i++)
        {
            var a = appointments[i]; var r = i + 4;
            ws.Cell(r, 1).Value = a.ScheduledAt.ToString("dd/MM/yyyy HH:mm");
            ws.Cell(r, 2).Value = a.Patient.Name;
            ws.Cell(r, 3).Value = a.Patient.Tutor.Name;
            ws.Cell(r, 4).Value = a.Veterinarian?.Name ?? "";
            ws.Cell(r, 5).Value = a.Type.ToString();
            ws.Cell(r, 6).Value = a.DurationMinutes;
            ws.Cell(r, 7).Value = a.Reason ?? "";
        }

        StyleData(ws, 4, 3 + appointments.Count, headers.Length);
        AutoFitAndFilter(ws, 3, headers.Length);

        // Aba Resumo
        var ws2 = wb.Worksheets.Add("Resumo");
        AddTitle(ws2, "Resumo de Atendimentos", clinicName);
        ws2.Cell(3, 1).Value = "Tipo"; ws2.Cell(3, 2).Value = "Quantidade";
        StyleHeader(ws2, 2);
        var groups = appointments.GroupBy(a => a.Type.ToString()).OrderByDescending(g => g.Count()).ToList();
        for (int i = 0; i < groups.Count; i++)
        {
            ws2.Cell(i + 4, 1).Value = groups[i].Key;
            ws2.Cell(i + 4, 2).Value = groups[i].Count();
        }
        AutoFitAndFilter(ws2, 3, 2);

        AddFooter(ws, 5 + appointments.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 8. PACIENTES =====
    public async Task<byte[]> ExportPatientsAsync(Guid clinicId, string clinicName, string userName,
        Guid? speciesId = null, bool? active = null)
    {
        var query = _db.Patients.Include(p => p.Tutor).Include(p => p.Species).Include(p => p.Breed)
            .Where(p => p.ClinicId == clinicId);
        if (speciesId.HasValue) query = query.Where(p => p.SpeciesId == speciesId.Value);
        if (active.HasValue) query = query.Where(p => p.Active == active.Value);

        var patients = await query.OrderBy(p => p.Name).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Pacientes");
        AddTitle(ws, "Cadastro de Pacientes", clinicName);

        var headers = new[] { "Nome", "Tutor", "Espécie", "Raça", "Sexo", "Nascimento", "Peso (kg)", "Microchip", "Castrado", "Ativo" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(3, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length);

        for (int i = 0; i < patients.Count; i++)
        {
            var p = patients[i]; var r = i + 4;
            ws.Cell(r, 1).Value = p.Name;
            ws.Cell(r, 2).Value = p.Tutor.Name;
            ws.Cell(r, 3).Value = p.Species.Name;
            ws.Cell(r, 4).Value = p.Breed?.Name ?? "SRD";
            ws.Cell(r, 5).Value = p.Sex.ToString();
            ws.Cell(r, 6).Value = p.BirthDate?.ToString("dd/MM/yyyy") ?? "";
            ws.Cell(r, 7).Value = p.Weight ?? 0;
            ws.Cell(r, 8).Value = p.Microchip ?? "";
            ws.Cell(r, 9).Value = p.Neutered ? "Sim" : "Não";
            ws.Cell(r, 10).Value = p.Active ? "Sim" : "Não";
        }

        StyleData(ws, 4, 3 + patients.Count, headers.Length);
        AutoFitAndFilter(ws, 3, headers.Length);
        AddFooter(ws, 5 + patients.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 9. TUTORES =====
    public async Task<byte[]> ExportTutorsAsync(Guid clinicId, string clinicName, string userName, bool? active = null)
    {
        var query = _db.Tutors.Include(t => t.Patients).Where(t => t.ClinicId == clinicId);
        if (active.HasValue) query = query.Where(t => t.Active == active.Value);
        var tutors = await query.OrderBy(t => t.Name).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Tutores");
        AddTitle(ws, "Cadastro de Tutores", clinicName);

        var headers = new[] { "Nome", "CPF", "Telefone", "E-mail", "Cidade", "UF", "Pacientes", "Ativo" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(3, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length);

        for (int i = 0; i < tutors.Count; i++)
        {
            var t = tutors[i]; var r = i + 4;
            ws.Cell(r, 1).Value = t.Name;
            ws.Cell(r, 2).Value = t.Cpf ?? "";
            ws.Cell(r, 3).Value = t.Phone ?? "";
            ws.Cell(r, 4).Value = t.Email ?? "";
            ws.Cell(r, 5).Value = t.City ?? "";
            ws.Cell(r, 6).Value = t.State ?? "";
            ws.Cell(r, 7).Value = t.Patients.Count;
            ws.Cell(r, 8).Value = t.Active ? "Sim" : "Não";
        }

        StyleData(ws, 4, 3 + tutors.Count, headers.Length);
        AutoFitAndFilter(ws, 3, headers.Length);
        AddFooter(ws, 5 + tutors.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 12. RECEITAS =====
    public async Task<byte[]> ExportFinancialRevenueAsync(Guid clinicId, string clinicName, string userName,
        DateTime? startDate = null, DateTime? endDate = null, Guid? categoryId = null)
    {
        return await ExportFinancialByTypeAsync(clinicId, clinicName, userName, TransactionType.Revenue,
            "Receitas", startDate, endDate, categoryId);
    }

    // ===== 13. DESPESAS =====
    public async Task<byte[]> ExportFinancialExpensesAsync(Guid clinicId, string clinicName, string userName,
        DateTime? startDate = null, DateTime? endDate = null, Guid? categoryId = null)
    {
        return await ExportFinancialByTypeAsync(clinicId, clinicName, userName, TransactionType.Expense,
            "Despesas", startDate, endDate, categoryId);
    }

    private async Task<byte[]> ExportFinancialByTypeAsync(Guid clinicId, string clinicName, string userName,
        TransactionType txType, string title, DateTime? startDate, DateTime? endDate, Guid? categoryId)
    {
        var query = _db.FinancialTransactions
            .Include(t => t.FinancialCategory).Include(t => t.Tutor).Include(t => t.CreatedBy)
            .Where(t => t.ClinicId == clinicId && t.Type == txType && t.Status != TransactionStatus.Cancelled);
        if (startDate.HasValue) query = query.Where(t => t.DueDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(t => t.DueDate <= endDate.Value);
        if (categoryId.HasValue) query = query.Where(t => t.FinancialCategoryId == categoryId.Value);

        var transactions = await query.OrderByDescending(t => t.DueDate).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(title);
        AddTitle(ws, $"Relatório de {title}", clinicName);

        var headers = new[] { "Data Venc.", "Descrição", "Categoria", "Tutor", "Valor", "Desconto", "Pago", "Status", "Método Pgto" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(3, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length);

        for (int i = 0; i < transactions.Count; i++)
        {
            var t = transactions[i]; var r = i + 4;
            ws.Cell(r, 1).Value = t.DueDate.ToString("dd/MM/yyyy");
            ws.Cell(r, 2).Value = t.Description;
            ws.Cell(r, 3).Value = t.FinancialCategory.Name;
            ws.Cell(r, 4).Value = t.Tutor?.Name ?? "";
            ws.Cell(r, 5).Value = t.Amount; ws.Cell(r, 5).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 6).Value = t.Discount ?? 0; ws.Cell(r, 6).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 7).Value = t.AmountPaid ?? 0; ws.Cell(r, 7).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 8).Value = t.Status.ToString();
            ws.Cell(r, 9).Value = t.PaymentMethod?.ToString() ?? "";
        }

        // Linha de total
        var totalRow = 4 + transactions.Count;
        ws.Cell(totalRow, 4).Value = "TOTAL";
        ws.Cell(totalRow, 4).Style.Font.Bold = true;
        ws.Cell(totalRow, 5).Value = transactions.Sum(t => t.Amount);
        ws.Cell(totalRow, 5).Style.NumberFormat.Format = "R$ #,##0.00";
        ws.Cell(totalRow, 5).Style.Font.Bold = true;
        ws.Range(totalRow, 1, totalRow, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml(TotalRowColor);

        StyleData(ws, 4, 3 + transactions.Count, headers.Length);
        AutoFitAndFilter(ws, 3, headers.Length);
        AddFooter(ws, totalRow + 2, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 14. FLUXO DE CAIXA =====
    public async Task<byte[]> ExportCashFlowAsync(Guid clinicId, string clinicName, string userName,
        DateTime startDate, DateTime endDate)
    {
        var transactions = await _db.FinancialTransactions
            .Where(t => t.ClinicId == clinicId && t.Status == TransactionStatus.Paid &&
                        t.PaidAt >= startDate && t.PaidAt <= endDate)
            .ToListAsync();

        var daily = transactions.GroupBy(t => t.PaidAt!.Value.Date)
            .Select(g => new { Date = g.Key,
                Revenue = g.Where(t => t.Type == TransactionType.Revenue).Sum(t => t.AmountPaid ?? t.Amount),
                Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.AmountPaid ?? t.Amount) })
            .OrderBy(x => x.Date).ToList();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Fluxo Diário");
        AddTitle(ws, "Fluxo de Caixa", clinicName, $"{startDate:dd/MM/yyyy} a {endDate:dd/MM/yyyy}");

        ws.Cell(3, 1).Value = "Data"; ws.Cell(3, 2).Value = "Receitas"; ws.Cell(3, 3).Value = "Despesas"; ws.Cell(3, 4).Value = "Saldo Diário"; ws.Cell(3, 5).Value = "Saldo Acumulado";
        StyleHeader(ws, 5);

        decimal cumulative = 0;
        for (int i = 0; i < daily.Count; i++)
        {
            var d = daily[i]; var r = i + 4;
            cumulative += d.Revenue - d.Expense;
            ws.Cell(r, 1).Value = d.Date.ToString("dd/MM/yyyy");
            ws.Cell(r, 2).Value = d.Revenue; ws.Cell(r, 2).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 3).Value = d.Expense; ws.Cell(r, 3).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 4).Value = d.Revenue - d.Expense; ws.Cell(r, 4).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 5).Value = cumulative; ws.Cell(r, 5).Style.NumberFormat.Format = "R$ #,##0.00";
            if (d.Revenue - d.Expense < 0) ws.Cell(r, 4).Style.Font.FontColor = XLColor.Red;
        }

        StyleData(ws, 4, 3 + daily.Count, 5);
        AutoFitAndFilter(ws, 3, 5);
        AddFooter(ws, 5 + daily.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 11. EXAMES =====
    public async Task<byte[]> ExportExamsAsync(Guid clinicId, string clinicName, string userName,
        DateTime? startDate = null, DateTime? endDate = null, Guid? examTypeId = null, ExamStatus? status = null)
    {
        var query = _db.ExamRequests
            .Include(e => e.Patient).Include(e => e.ExamType).Include(e => e.RequestedBy)
            .Where(e => e.ClinicId == clinicId);
        if (startDate.HasValue) query = query.Where(e => e.RequestedAt >= startDate.Value);
        if (endDate.HasValue) query = query.Where(e => e.RequestedAt <= endDate.Value);
        if (examTypeId.HasValue) query = query.Where(e => e.ExamTypeId == examTypeId.Value);
        if (status.HasValue) query = query.Where(e => e.Status == status.Value);

        var exams = await query.OrderByDescending(e => e.RequestedAt).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Exames");
        AddTitle(ws, "Relatório de Exames", clinicName);

        var headers = new[] { "Data Solicitação", "Paciente", "Tipo Exame", "Solicitado Por", "Status", "Data Coleta", "Data Conclusão" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(3, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length);

        for (int i = 0; i < exams.Count; i++)
        {
            var e = exams[i]; var r = i + 4;
            ws.Cell(r, 1).Value = e.RequestedAt.ToString("dd/MM/yyyy");
            ws.Cell(r, 2).Value = e.Patient.Name;
            ws.Cell(r, 3).Value = e.ExamType.Name;
            ws.Cell(r, 4).Value = e.RequestedBy.Name;
            ws.Cell(r, 5).Value = e.Status.ToString();
            ws.Cell(r, 6).Value = e.CollectedAt?.ToString("dd/MM/yyyy") ?? "";
            ws.Cell(r, 7).Value = e.CompletedAt?.ToString("dd/MM/yyyy") ?? "";
        }

        StyleData(ws, 4, 3 + exams.Count, headers.Length);
        AutoFitAndFilter(ws, 3, headers.Length);
        AddFooter(ws, 5 + exams.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 10. INTERNAÇÕES =====
    public async Task<byte[]> ExportHospitalizationsAsync(Guid clinicId, string clinicName, string userName,
        DateTime? startDate = null, DateTime? endDate = null, HospitalizationStatus? status = null)
    {
        var query = _db.Hospitalizations
            .Include(h => h.Patient).ThenInclude(p => p.Tutor)
            .Include(h => h.Veterinarian)
            .Where(h => h.ClinicId == clinicId);
        if (startDate.HasValue) query = query.Where(h => h.AdmittedAt >= startDate.Value);
        if (endDate.HasValue) query = query.Where(h => h.AdmittedAt <= endDate.Value);
        if (status.HasValue) query = query.Where(h => h.Status == status.Value);

        var hosps = await query.OrderByDescending(h => h.AdmittedAt).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Internações");
        AddTitle(ws, "Relatório de Internações", clinicName);

        var headers = new[] { "Admissão", "Paciente", "Tutor", "Veterinário", "Motivo", "Baia", "Alta", "Status" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(3, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length);

        for (int i = 0; i < hosps.Count; i++)
        {
            var h = hosps[i]; var r = i + 4;
            ws.Cell(r, 1).Value = h.AdmittedAt.ToString("dd/MM/yyyy");
            ws.Cell(r, 2).Value = h.Patient.Name;
            ws.Cell(r, 3).Value = h.Patient.Tutor.Name;
            ws.Cell(r, 4).Value = h.Veterinarian.Name;
            ws.Cell(r, 5).Value = h.Reason ?? "";
            ws.Cell(r, 6).Value = h.Cage ?? "";
            ws.Cell(r, 7).Value = h.DischargedAt?.ToString("dd/MM/yyyy") ?? "";
            ws.Cell(r, 8).Value = h.Status.ToString();
        }

        StyleData(ws, 4, 3 + hosps.Count, headers.Length);
        AutoFitAndFilter(ws, 3, headers.Length);
        AddFooter(ws, 5 + hosps.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 18. CENTROS DE CUSTO =====
    public async Task<byte[]> ExportCostCentersAsync(Guid clinicId, string clinicName, string userName,
        DateTime? startDate = null, DateTime? endDate = null)
    {
        var centers = await _db.CostCenters.Include(c => c.Transactions)
            .Where(c => c.ClinicId == clinicId).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Custos por Centro");
        AddTitle(ws, "Relatório por Centro de Custo", clinicName);

        ws.Cell(3, 1).Value = "Centro de Custo"; ws.Cell(3, 2).Value = "Receitas"; ws.Cell(3, 3).Value = "Despesas"; ws.Cell(3, 4).Value = "Saldo"; ws.Cell(3, 5).Value = "Transações";
        StyleHeader(ws, 5);

        for (int i = 0; i < centers.Count; i++)
        {
            var c = centers[i]; var r = i + 4;
            var txs = c.Transactions.Where(t => t.Status != TransactionStatus.Cancelled);
            if (startDate.HasValue) txs = txs.Where(t => t.DueDate >= startDate.Value);
            if (endDate.HasValue) txs = txs.Where(t => t.DueDate <= endDate.Value);
            var list = txs.ToList();
            var rev = list.Where(t => t.Type == TransactionType.Revenue).Sum(t => t.Amount);
            var exp = list.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            ws.Cell(r, 1).Value = c.Name;
            ws.Cell(r, 2).Value = rev; ws.Cell(r, 2).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 3).Value = exp; ws.Cell(r, 3).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 4).Value = rev - exp; ws.Cell(r, 4).Style.NumberFormat.Format = "R$ #,##0.00";
            if (rev - exp < 0) ws.Cell(r, 4).Style.Font.FontColor = XLColor.Red;
            ws.Cell(r, 5).Value = list.Count;
        }

        StyleData(ws, 4, 3 + centers.Count, 5);
        AutoFitAndFilter(ws, 3, 5);
        AddFooter(ws, 5 + centers.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 23. AUDIT LOGS =====
    public async Task<byte[]> ExportAuditLogsAsync(Guid clinicId, string clinicName, string userName,
        Guid? userId = null, string? entity = null, string? action = null,
        DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _db.AuditLogs.Include(a => a.User).Where(a => a.ClinicId == clinicId);
        if (userId.HasValue) query = query.Where(a => a.UserId == userId.Value);
        if (!string.IsNullOrEmpty(entity)) query = query.Where(a => a.Entity == entity);
        if (!string.IsNullOrEmpty(action)) query = query.Where(a => a.Action == action);
        if (startDate.HasValue) query = query.Where(a => a.CreatedAt >= startDate.Value);
        if (endDate.HasValue) query = query.Where(a => a.CreatedAt <= endDate.Value);

        var logs = await query.OrderByDescending(a => a.CreatedAt).Take(10000).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Logs");
        AddTitle(ws, "Logs de Auditoria", clinicName);

        var headers = new[] { "Data/Hora", "Usuário", "Ação", "Entidade", "ID Entidade", "IP" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(3, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length);

        for (int i = 0; i < logs.Count; i++)
        {
            var l = logs[i]; var r = i + 4;
            ws.Cell(r, 1).Value = l.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
            ws.Cell(r, 2).Value = l.User.Name;
            ws.Cell(r, 3).Value = l.Action;
            ws.Cell(r, 4).Value = l.Entity;
            ws.Cell(r, 5).Value = l.EntityId;
            ws.Cell(r, 6).Value = l.IpAddress ?? "";
        }

        StyleData(ws, 4, 3 + logs.Count, headers.Length);
        AutoFitAndFilter(ws, 3, headers.Length);
        AddFooter(ws, 5 + logs.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== EXPORT PRODUTOS (inline) =====
    public async Task<byte[]> ExportProductsAsync(Guid clinicId, string clinicName, string userName,
        string? search = null, Guid? categoryId = null)
    {
        return await ExportInventoryAsync(clinicId, clinicName, userName, categoryId, null);
    }

    // ===== EXPORT PEDIDO INDIVIDUAL =====
    public async Task<byte[]> ExportOrderAsync(Guid orderId, Guid clinicId, string clinicName, string userName)
    {
        var order = await _db.Orders
            .Include(o => o.CreatedBy).Include(o => o.ApprovedBy)
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Unit)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.ClinicId == clinicId);

        if (order == null) return [];

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Pedido");
        AddTitle(ws, $"Pedido {order.Code}", clinicName);

        ws.Cell(3, 1).Value = "Código:"; ws.Cell(3, 2).Value = order.Code;
        ws.Cell(4, 1).Value = "Tipo:"; ws.Cell(4, 2).Value = order.Type.ToString();
        ws.Cell(5, 1).Value = "Status:"; ws.Cell(5, 2).Value = order.Status.ToString();
        ws.Cell(6, 1).Value = "Criado por:"; ws.Cell(6, 2).Value = order.CreatedBy.Name;
        ws.Cell(7, 1).Value = "Aprovado por:"; ws.Cell(7, 2).Value = order.ApprovedBy?.Name ?? "—";
        ws.Range(3, 1, 7, 1).Style.Font.Bold = true;

        var headers = new[] { "Produto", "Unidade", "Qtd. Solicitada", "Qtd. Aprovada", "Qtd. Recebida", "Observações" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(9, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length, 9);

        for (int i = 0; i < order.Items.Count; i++)
        {
            var it = order.Items.ElementAt(i); var r = i + 10;
            ws.Cell(r, 1).Value = it.Product.Name;
            ws.Cell(r, 2).Value = it.Product.Unit.Abbreviation;
            ws.Cell(r, 3).Value = it.QuantityRequested;
            ws.Cell(r, 4).Value = it.QuantityApproved ?? 0;
            ws.Cell(r, 5).Value = it.QuantityReceived;
            ws.Cell(r, 6).Value = it.Notes ?? "";
        }

        StyleData(ws, 10, 9 + order.Items.Count, headers.Length);
        AutoFitAndFilter(ws, 9, headers.Length);
        AddFooter(ws, 11 + order.Items.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 15. INADIMPLÊNCIA =====
    public async Task<byte[]> ExportOverdueAsync(Guid clinicId, string clinicName, string userName)
    {
        var transactions = await _db.FinancialTransactions
            .Include(t => t.FinancialCategory).Include(t => t.Tutor)
            .Where(t => t.ClinicId == clinicId && t.Status == TransactionStatus.Pending && t.DueDate < DateTime.UtcNow)
            .OrderBy(t => t.DueDate).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Contas Vencidas");
        AddTitle(ws, "Inadimplência", clinicName);

        var headers = new[] { "Vencimento", "Descrição", "Categoria", "Tutor", "Valor", "Dias Atraso" };
        for (int i = 0; i < headers.Length; i++) ws.Cell(3, i + 1).Value = headers[i];
        StyleHeader(ws, headers.Length);

        for (int i = 0; i < transactions.Count; i++)
        {
            var t = transactions[i]; var r = i + 4;
            ws.Cell(r, 1).Value = t.DueDate.ToString("dd/MM/yyyy");
            ws.Cell(r, 2).Value = t.Description;
            ws.Cell(r, 3).Value = t.FinancialCategory.Name;
            ws.Cell(r, 4).Value = t.Tutor?.Name ?? "";
            ws.Cell(r, 5).Value = t.Amount; ws.Cell(r, 5).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 6).Value = (DateTime.UtcNow - t.DueDate).Days;
            ws.Cell(r, 5).Style.Font.FontColor = XLColor.Red;
        }

        StyleData(ws, 4, 3 + transactions.Count, headers.Length);
        AutoFitAndFilter(ws, 3, headers.Length);
        AddFooter(ws, 5 + transactions.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 16. POR CATEGORIA FINANCEIRA =====
    public async Task<byte[]> ExportByFinancialCategoryAsync(Guid clinicId, string clinicName, string userName,
        DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _db.FinancialTransactions
            .Include(t => t.FinancialCategory)
            .Where(t => t.ClinicId == clinicId && t.Status != TransactionStatus.Cancelled);
        if (startDate.HasValue) query = query.Where(t => t.DueDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(t => t.DueDate <= endDate.Value);

        var transactions = await query.ToListAsync();

        using var wb = new XLWorkbook();

        foreach (var txType in new[] { TransactionType.Revenue, TransactionType.Expense })
        {
            var sheetName = txType == TransactionType.Revenue ? "Receitas por Cat." : "Despesas por Cat.";
            var ws = wb.Worksheets.Add(sheetName);
            AddTitle(ws, sheetName, clinicName);
            ws.Cell(3, 1).Value = "Categoria"; ws.Cell(3, 2).Value = "Quantidade"; ws.Cell(3, 3).Value = "Total";
            StyleHeader(ws, 3);

            var groups = transactions.Where(t => t.Type == txType)
                .GroupBy(t => t.FinancialCategory.Name)
                .OrderByDescending(g => g.Sum(t => t.Amount)).ToList();

            for (int i = 0; i < groups.Count; i++)
            {
                var g = groups[i]; var r = i + 4;
                ws.Cell(r, 1).Value = g.Key;
                ws.Cell(r, 2).Value = g.Count();
                ws.Cell(r, 3).Value = g.Sum(t => t.Amount); ws.Cell(r, 3).Style.NumberFormat.Format = "R$ #,##0.00";
            }
            AutoFitAndFilter(ws, 3, 3);
        }

        AddFooter(wb.Worksheets.First(), wb.Worksheets.First().LastRowUsed()!.RowNumber() + 2, userName, clinicName);
        return WorkbookToBytes(wb);
    }

    // ===== 17. POR TUTOR =====
    public async Task<byte[]> ExportByTutorAsync(Guid clinicId, string clinicName, string userName,
        DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _db.FinancialTransactions
            .Include(t => t.Tutor)
            .Where(t => t.ClinicId == clinicId && t.TutorId != null && t.Type == TransactionType.Revenue
                        && t.Status != TransactionStatus.Cancelled);
        if (startDate.HasValue) query = query.Where(t => t.DueDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(t => t.DueDate <= endDate.Value);

        var transactions = await query.ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Faturamento por Tutor");
        AddTitle(ws, "Faturamento por Tutor", clinicName);

        ws.Cell(3, 1).Value = "Tutor"; ws.Cell(3, 2).Value = "Transações"; ws.Cell(3, 3).Value = "Total"; ws.Cell(3, 4).Value = "Pago"; ws.Cell(3, 5).Value = "Pendente";
        StyleHeader(ws, 5);

        var groups = transactions.GroupBy(t => t.Tutor!.Name).OrderByDescending(g => g.Sum(t => t.Amount)).ToList();
        for (int i = 0; i < groups.Count; i++)
        {
            var g = groups[i]; var r = i + 4;
            ws.Cell(r, 1).Value = g.Key;
            ws.Cell(r, 2).Value = g.Count();
            ws.Cell(r, 3).Value = g.Sum(t => t.Amount); ws.Cell(r, 3).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 4).Value = g.Where(t => t.Status == TransactionStatus.Paid).Sum(t => t.AmountPaid ?? t.Amount);
            ws.Cell(r, 4).Style.NumberFormat.Format = "R$ #,##0.00";
            ws.Cell(r, 5).Value = g.Where(t => t.Status == TransactionStatus.Pending).Sum(t => t.Amount);
            ws.Cell(r, 5).Style.NumberFormat.Format = "R$ #,##0.00";
        }

        AutoFitAndFilter(ws, 3, 5);
        AddFooter(ws, 5 + groups.Count, userName, clinicName);
        return WorkbookToBytes(wb);
    }
}
