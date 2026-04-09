using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Comum;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Authorize]
public class ExamesController : ControllerBase
{
    private readonly ServicoExame _servico;
    public ExamesController(ServicoExame servico) { _servico = servico; }
    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;
    private Guid UsuarioId => (Guid)HttpContext.Items["UsuarioId"]!;

    [HttpGet("api/tipos-exame")]
    public async Task<IActionResult> ListarTipos() => Ok(await _servico.ListarTiposAsync());

    [HttpPost("api/tipos-exame")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CriarTipo([FromBody] CriarTipoExameDto dto) =>
        Created("", await _servico.CriarTipoAsync(dto.Nome, dto.Categoria, dto.PrecoDefault));

    [HttpPut("api/tipos-exame/{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> AtualizarTipo(Guid id, [FromBody] CriarTipoExameDto dto)
    {
        var t = await _servico.AtualizarTipoAsync(id, dto.Nome, dto.Categoria, dto.PrecoDefault);
        return t == null ? NotFound() : Ok(t);
    }

    [HttpGet("api/solicitacoes-exame")]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20, [FromQuery] StatusExame? status = null, [FromQuery] Guid? pacienteId = null, [FromQuery] Guid? tipoExameId = null, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var (itens, total) = await _servico.ListarSolicitacoesPaginadoAsync(ClinicaId, pagina, tamanhoPagina, status, pacienteId, tipoExameId, dataInicio, dataFim);
        return Ok(new RespostaPaginada<object>
        {
            Itens = itens.Select(e => new { e.Id, e.PacienteId, nomePaciente = e.Paciente.Nome, nomeTutor = e.Paciente.Tutor.Nome, e.SolicitadoPorId, nomeSolicitante = e.SolicitadoPor.Nome, e.TipoExameId, nomeTipoExame = e.TipoExame.Nome, categoriaTipoExame = e.TipoExame.Categoria, status = e.Status.ToString(), e.IndicacaoClinica, e.Observacoes, e.DataSolicitacao, e.DataColeta, e.DataConclusao, e.CriadoEm, resultado = e.Resultado != null ? new { e.Resultado.Id, e.Resultado.RealizadoPorId, nomeRealizadoPor = e.Resultado.RealizadoPor.Nome, e.Resultado.TextoResultado, e.Resultado.ArquivoResultadoUrl, e.Resultado.Conclusao, e.Resultado.CriadoEm } : null } as object).ToList(),
            TotalRegistros = total, Pagina = pagina, TamanhoPagina = tamanhoPagina
        });
    }

    [HttpGet("api/solicitacoes-exame/{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var e = await _servico.ObterSolicitacaoPorIdAsync(id, ClinicaId);
        return e == null ? NotFound() : Ok(e);
    }

    [HttpPost("api/solicitacoes-exame")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinario")]
    public async Task<IActionResult> Criar([FromBody] CriarSolicitacaoExameDto dto)
    {
        var e = await _servico.CriarSolicitacaoAsync(ClinicaId, UsuarioId, dto.PacienteId, dto.TipoExameId, dto.ProntuarioId, dto.IndicacaoClinica, dto.Observacoes);
        return CreatedAtAction(nameof(ObterPorId), new { id = e.Id }, e);
    }

    [HttpPatch("api/solicitacoes-exame/{id:guid}/coletar")]
    public async Task<IActionResult> Coletar(Guid id)
    {
        try { var e = await _servico.ColetarAsync(id, ClinicaId); return e == null ? NotFound() : Ok(e); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpPatch("api/solicitacoes-exame/{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        try { var e = await _servico.CancelarAsync(id, ClinicaId); return e == null ? NotFound() : Ok(e); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpPost("api/solicitacoes-exame/{id:guid}/resultado")]
    public async Task<IActionResult> AdicionarResultado(Guid id, [FromBody] CriarResultadoExameDto dto)
    {
        try
        {
            var r = await _servico.AdicionarResultadoAsync(id, ClinicaId, UsuarioId, dto.TextoResultado, dto.ArquivoResultadoUrl, dto.ValoresReferencia, dto.Observacoes, dto.Conclusao);
            return Created("", r);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpGet("api/solicitacoes-exame/{id:guid}/resultado")]
    public async Task<IActionResult> ObterResultado(Guid id)
    {
        var r = await _servico.ObterResultadoAsync(id, ClinicaId);
        return r == null ? NotFound() : Ok(r);
    }
}

public record CriarTipoExameDto(string Nome, string? Categoria = null, decimal? PrecoDefault = null);
public record CriarSolicitacaoExameDto(Guid PacienteId, Guid TipoExameId, Guid? ProntuarioId = null, string? IndicacaoClinica = null, string? Observacoes = null);
public record CriarResultadoExameDto(string? TextoResultado = null, string? ArquivoResultadoUrl = null, string? ValoresReferencia = null, string? Observacoes = null, string? Conclusao = null);
