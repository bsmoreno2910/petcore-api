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
}
