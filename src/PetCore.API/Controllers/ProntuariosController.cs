using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Comum;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/prontuarios")]
[Authorize]
public class ProntuariosController : ControllerBase
{
    private readonly ServicoProntuario _servico;
    public ProntuariosController(ServicoProntuario servico) { _servico = servico; }
    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;
    private Guid UsuarioId => (Guid)HttpContext.Items["UsuarioId"]!;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20, [FromQuery] Guid? pacienteId = null, [FromQuery] Guid? veterinarioId = null, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var (itens, total) = await _servico.ListarPaginadoAsync(ClinicaId, pagina, tamanhoPagina, pacienteId, veterinarioId, dataInicio, dataFim);
        return Ok(new RespostaPaginada<object>
        {
            Itens = itens.Select(MapearProntuario).ToList(),
            TotalRegistros = total, Pagina = pagina, TamanhoPagina = tamanhoPagina
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var p = await _servico.ObterPorIdAsync(id, ClinicaId);
        return p == null ? NotFound() : Ok(MapearProntuario(p));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinario")]
    public async Task<IActionResult> Criar([FromBody] CriarProntuarioDto dto)
    {
        var prontuario = new Prontuario
        {
            PacienteId = dto.PacienteId, AgendamentoId = dto.AgendamentoId,
            QueixaPrincipal = dto.QueixaPrincipal, Historico = dto.Historico, Anamnese = dto.Anamnese,
            Peso = dto.Peso, Temperatura = dto.Temperatura, FrequenciaCardiaca = dto.FrequenciaCardiaca,
            FrequenciaRespiratoria = dto.FrequenciaRespiratoria, ExameFisico = dto.ExameFisico,
            Mucosas = dto.Mucosas, Hidratacao = dto.Hidratacao, Linfonodos = dto.Linfonodos,
            Diagnostico = dto.Diagnostico, DiagnosticoDiferencial = dto.DiagnosticoDiferencial,
            Tratamento = dto.Tratamento, Observacoes = dto.Observacoes, NotasInternas = dto.NotasInternas
        };
        var criado = await _servico.CriarAsync(prontuario, ClinicaId, UsuarioId);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, MapearProntuario(criado));
    }

    [HttpPost("{id:guid}/prescricoes")]
    [Authorize(Roles = "SuperAdmin,Veterinario")]
    public async Task<IActionResult> AdicionarPrescricao(Guid id, [FromBody] CriarPrescricaoDto dto)
    {
        try
        {
            var presc = new Prescricao { NomeMedicamento = dto.NomeMedicamento, Dosagem = dto.Dosagem, Frequencia = dto.Frequencia, Duracao = dto.Duracao, ViaAdministracao = dto.ViaAdministracao, Instrucoes = dto.Instrucoes, Quantidade = dto.Quantidade };
            var criada = await _servico.AdicionarPrescricaoAsync(id, ClinicaId, presc);
            return Created("", new { criada.Id, criada.NomeMedicamento, criada.Dosagem, criada.Frequencia });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpDelete("{id:guid}/prescricoes/{prescId:guid}")]
    [Authorize(Roles = "SuperAdmin,Veterinario")]
    public async Task<IActionResult> RemoverPrescricao(Guid id, Guid prescId) =>
        await _servico.RemoverPrescricaoAsync(id, prescId, ClinicaId) ? NoContent() : NotFound();

    private static object MapearProntuario(Prontuario p) => new
    {
        p.Id, p.ClinicaId, p.PacienteId, nomePaciente = p.Paciente.Nome,
        p.VeterinarioId, nomeVeterinario = p.Veterinario.Nome, p.AgendamentoId,
        p.QueixaPrincipal, p.Historico, p.Anamnese, p.Peso, p.Temperatura,
        p.FrequenciaCardiaca, p.FrequenciaRespiratoria, p.ExameFisico, p.Mucosas,
        p.Hidratacao, p.Linfonodos, p.Diagnostico, p.DiagnosticoDiferencial,
        p.Tratamento, p.Observacoes, p.NotasInternas, p.CriadoEm, p.AtualizadoEm,
        prescricoes = p.Prescricoes.Select(pr => new { pr.Id, pr.ProntuarioId, pr.NomeMedicamento, pr.Dosagem, pr.Frequencia, pr.Duracao, pr.ViaAdministracao, pr.Instrucoes, pr.Quantidade, pr.CriadoEm })
    };
}

public record CriarProntuarioDto(Guid PacienteId, Guid? AgendamentoId = null, string? QueixaPrincipal = null, string? Historico = null, string? Anamnese = null, decimal? Peso = null, decimal? Temperatura = null, int? FrequenciaCardiaca = null, int? FrequenciaRespiratoria = null, string? ExameFisico = null, string? Mucosas = null, string? Hidratacao = null, string? Linfonodos = null, string? Diagnostico = null, string? DiagnosticoDiferencial = null, string? Tratamento = null, string? Observacoes = null, string? NotasInternas = null);
public record CriarPrescricaoDto(string NomeMedicamento, string? Dosagem = null, string? Frequencia = null, string? Duracao = null, string? ViaAdministracao = null, string? Instrucoes = null, int? Quantidade = null);
