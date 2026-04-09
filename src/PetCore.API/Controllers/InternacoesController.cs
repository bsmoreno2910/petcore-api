using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Comum;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/internacoes")]
[Authorize]
public class InternacoesController : ControllerBase
{
    private readonly ServicoInternacao _servico;
    public InternacoesController(ServicoInternacao servico) { _servico = servico; }
    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;
    private Guid UsuarioId => (Guid)HttpContext.Items["UsuarioId"]!;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20, [FromQuery] StatusInternacao? status = null, [FromQuery] Guid? pacienteId = null)
    {
        var (itens, total) = await _servico.ListarPaginadoAsync(ClinicaId, pagina, tamanhoPagina, status, pacienteId);
        return Ok(new RespostaPaginada<object>
        {
            Itens = itens.Select(h => new { h.Id, h.PacienteId, nomePaciente = h.Paciente.Nome, nomeTutor = h.Paciente.Tutor.Nome, nomeEspecie = h.Paciente.Especie.Nome, h.VeterinarioId, nomeVeterinario = h.Veterinario.Nome, status = h.Status.ToString(), h.Motivo, h.Baia, h.Dieta, h.Observacoes, h.DataInternacao, h.DataAlta, h.ObservacoesAlta, h.CriadoEm, quantidadeEvolucoes = h.Evolucoes.Count } as object).ToList(),
            TotalRegistros = total, Pagina = pagina, TamanhoPagina = tamanhoPagina
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var h = await _servico.ObterPorIdAsync(id, ClinicaId);
        return h == null ? NotFound() : Ok(h);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinario")]
    public async Task<IActionResult> Criar([FromBody] CriarInternacaoDto dto)
    {
        var h = await _servico.CriarAsync(ClinicaId, UsuarioId, dto.PacienteId, dto.Motivo, dto.Baia, dto.Dieta, dto.Observacoes, dto.DataInternacao);
        return CreatedAtAction(nameof(ObterPorId), new { id = h.Id }, h);
    }

    [HttpPatch("{id:guid}/dar-alta")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinario")]
    public async Task<IActionResult> DarAlta(Guid id, [FromBody] AltaDto? dto)
    {
        try { var h = await _servico.DarAltaAsync(id, ClinicaId, dto?.ObservacoesAlta); return h == null ? NotFound() : Ok(h); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpPost("{id:guid}/evolucoes")]
    [Authorize(Roles = "SuperAdmin,Veterinario")]
    public async Task<IActionResult> AdicionarEvolucao(Guid id, [FromBody] CriarEvolucaoDto dto)
    {
        try
        {
            var ev = new Evolucao { Peso = dto.Peso, Temperatura = dto.Temperatura, FrequenciaCardiaca = dto.FrequenciaCardiaca, FrequenciaRespiratoria = dto.FrequenciaRespiratoria, Descricao = dto.Descricao, Medicamentos = dto.Medicamentos, Alimentacao = dto.Alimentacao, Observacoes = dto.Observacoes };
            var criada = await _servico.AdicionarEvolucaoAsync(id, ClinicaId, UsuarioId, ev);
            return Created("", criada);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpGet("{id:guid}/evolucoes")]
    public async Task<IActionResult> ListarEvolucoes(Guid id) => Ok(await _servico.ListarEvolucoesAsync(id, ClinicaId));
}

public record CriarInternacaoDto(Guid PacienteId, string? Motivo = null, string? Baia = null, string? Dieta = null, string? Observacoes = null, DateTime? DataInternacao = null);
public record AltaDto(string? ObservacoesAlta = null);
public record CriarEvolucaoDto(decimal? Peso = null, decimal? Temperatura = null, int? FrequenciaCardiaca = null, int? FrequenciaRespiratoria = null, string? Descricao = null, string? Medicamentos = null, string? Alimentacao = null, string? Observacoes = null);
