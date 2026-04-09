using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCore.API.DTOs.Comum;
using PetCore.Infrastructure.Data;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/logs-auditoria")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class LogsAuditoriaController : ControllerBase
{
    private readonly AppDbContext _db;
    public LogsAuditoriaController(AppDbContext db) { _db = db; }

    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20,
        [FromQuery] string? entidade = null,
        [FromQuery] string? acao = null,
        [FromQuery] Guid? usuarioId = null,
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        var query = _db.LogsAuditoria
            .Include(l => l.Usuario)
            .Where(l => l.ClinicaId == ClinicaId);

        if (!string.IsNullOrWhiteSpace(entidade))
            query = query.Where(l => l.Entidade == entidade);

        if (!string.IsNullOrWhiteSpace(acao))
            query = query.Where(l => l.Acao == acao);

        if (usuarioId.HasValue)
            query = query.Where(l => l.UsuarioId == usuarioId.Value);

        if (dataInicio.HasValue)
            query = query.Where(l => l.CriadoEm >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(l => l.CriadoEm <= dataFim.Value);

        var total = await query.CountAsync();

        var itens = await query
            .OrderByDescending(l => l.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(l => new
            {
                l.Id,
                l.ClinicaId,
                l.UsuarioId,
                nomeUsuario = l.Usuario.Nome,
                l.Acao,
                l.Entidade,
                l.EntidadeId,
                l.ValorAntigo,
                l.NovoValor,
                l.CriadoEm
            })
            .ToListAsync();

        return Ok(new RespostaPaginada<object>
        {
            Itens = itens.Cast<object>().ToList(),
            TotalRegistros = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        });
    }

    [HttpGet("exportar")]
    public async Task<IActionResult> Exportar(
        [FromQuery] string? entidade = null,
        [FromQuery] string? acao = null,
        [FromQuery] Guid? usuarioId = null,
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        var query = _db.LogsAuditoria
            .Include(l => l.Usuario)
            .Where(l => l.ClinicaId == ClinicaId);

        if (!string.IsNullOrWhiteSpace(entidade))
            query = query.Where(l => l.Entidade == entidade);

        if (!string.IsNullOrWhiteSpace(acao))
            query = query.Where(l => l.Acao == acao);

        if (usuarioId.HasValue)
            query = query.Where(l => l.UsuarioId == usuarioId.Value);

        if (dataInicio.HasValue)
            query = query.Where(l => l.CriadoEm >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(l => l.CriadoEm <= dataFim.Value);

        var logs = await query
            .OrderByDescending(l => l.CriadoEm)
            .Select(l => new
            {
                l.CriadoEm,
                NomeUsuario = l.Usuario.Nome,
                l.Acao,
                l.Entidade,
                l.EntidadeId,
                l.ValorAntigo,
                l.NovoValor,
                l.EnderecoIp
            })
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Auditoria");

        ws.Cell(1, 1).Value = "Data";
        ws.Cell(1, 2).Value = "Usuário";
        ws.Cell(1, 3).Value = "Ação";
        ws.Cell(1, 4).Value = "Entidade";
        ws.Cell(1, 5).Value = "ID Entidade";
        ws.Cell(1, 6).Value = "Valor Antigo";
        ws.Cell(1, 7).Value = "Novo Valor";
        ws.Cell(1, 8).Value = "IP";

        var headerRow = ws.Row(1);
        headerRow.Style.Font.Bold = true;

        for (int i = 0; i < logs.Count; i++)
        {
            var row = i + 2;
            ws.Cell(row, 1).Value = logs[i].CriadoEm.ToString("dd/MM/yyyy HH:mm:ss");
            ws.Cell(row, 2).Value = logs[i].NomeUsuario;
            ws.Cell(row, 3).Value = logs[i].Acao;
            ws.Cell(row, 4).Value = logs[i].Entidade;
            ws.Cell(row, 5).Value = logs[i].EntidadeId;
            ws.Cell(row, 6).Value = logs[i].ValorAntigo ?? "";
            ws.Cell(row, 7).Value = logs[i].NovoValor ?? "";
            ws.Cell(row, 8).Value = logs[i].EnderecoIp ?? "";
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);

        return File(
            ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "PetCore_AuditLogs.xlsx");
    }
}
