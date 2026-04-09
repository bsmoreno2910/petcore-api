using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Comum;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/agendamentos")]
[Authorize]
public class AgendamentosController : ControllerBase
{
    private readonly ServicoAgendamento _servico;
    public AgendamentosController(ServicoAgendamento servico) { _servico = servico; }

    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20, [FromQuery] DateTime? data = null, [FromQuery] Guid? veterinarioId = null, [FromQuery] StatusAgendamento? status = null, [FromQuery] TipoAgendamento? tipo = null)
    {
        var (itens, total) = await _servico.ListarPaginadoAsync(ClinicaId, pagina, tamanhoPagina, data, veterinarioId, status, tipo);
        return Ok(new RespostaPaginada<object>
        {
            Itens = itens.Select(a => MapearAgendamento(a) as object).ToList(),
            TotalRegistros = total, Pagina = pagina, TamanhoPagina = tamanhoPagina
        });
    }

    [HttpGet("calendario")]
    public async Task<IActionResult> Calendario([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
    {
        var agendamentos = await _servico.ListarCalendarioAsync(ClinicaId, dataInicio, dataFim);
        return Ok(agendamentos.Select(a => new
        {
            a.Id, titulo = $"{a.Paciente.Nome} - {a.Tipo}", inicio = a.DataHoraAgendada,
            fim = a.DataHoraAgendada.AddMinutes(a.DuracaoMinutos),
            cor = CorPorTipo(a.Tipo), tipo = a.Tipo.ToString(), status = a.Status.ToString(),
            a.PacienteId, a.VeterinarioId
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var a = await _servico.ObterPorIdAsync(id, ClinicaId);
        return a == null ? NotFound() : Ok(MapearAgendamento(a));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarAgendamentoDto dto)
    {
        if (!Enum.TryParse<TipoAgendamento>(dto.Tipo, true, out var tipo)) return BadRequest(new { erro = "Tipo inválido." });
        try
        {
            var a = await _servico.CriarAsync(ClinicaId, dto.PacienteId, dto.VeterinarioId, tipo, dto.DataHoraAgendada, dto.DuracaoMinutos, dto.Motivo, dto.Observacoes);
            return CreatedAtAction(nameof(ObterPorId), new { id = a.Id }, MapearAgendamento(a));
        }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpPatch("{id:guid}/confirmar")]
    public Task<IActionResult> Confirmar(Guid id) => AlterarStatus(id, StatusAgendamento.Confirmado);
    [HttpPatch("{id:guid}/checkin")]
    public Task<IActionResult> CheckIn(Guid id) => AlterarStatus(id, StatusAgendamento.Chegou);
    [HttpPatch("{id:guid}/iniciar")]
    public Task<IActionResult> Iniciar(Guid id) => AlterarStatus(id, StatusAgendamento.EmAndamento);
    [HttpPatch("{id:guid}/concluir")]
    public Task<IActionResult> Concluir(Guid id) => AlterarStatus(id, StatusAgendamento.Concluido);
    [HttpPatch("{id:guid}/faltou")]
    public Task<IActionResult> Faltou(Guid id) => AlterarStatus(id, StatusAgendamento.Faltou);

    [HttpPatch("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarAgendamentoDto? dto)
    {
        try
        {
            var a = await _servico.AlterarStatusAsync(id, ClinicaId, StatusAgendamento.Cancelado, dto?.MotivoCancelamento);
            return a == null ? NotFound() : Ok(MapearAgendamento(a));
        }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpGet("horarios-disponiveis")]
    public async Task<IActionResult> HorariosDisponiveis([FromQuery] Guid veterinarioId, [FromQuery] DateTime data, [FromQuery] int duracao = 30)
    {
        var slots = await _servico.ListarHorariosDisponiveisAsync(ClinicaId, veterinarioId, data, duracao);
        return Ok(slots.Select(s => new { inicio = s.Inicio, fim = s.Fim }));
    }

    private async Task<IActionResult> AlterarStatus(Guid id, StatusAgendamento novoStatus)
    {
        try
        {
            var a = await _servico.AlterarStatusAsync(id, ClinicaId, novoStatus);
            return a == null ? NotFound() : Ok(MapearAgendamento(a));
        }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    private static object MapearAgendamento(Domain.Entities.Agendamento a) => new
    {
        a.Id, a.ClinicaId, a.PacienteId, nomePaciente = a.Paciente.Nome, nomeTutor = a.Paciente.Tutor.Nome,
        telefoneTutor = a.Paciente.Tutor.Telefone, nomeEspecie = a.Paciente.Especie.Nome,
        a.VeterinarioId, nomeVeterinario = a.Veterinario?.Nome, tipo = a.Tipo.ToString(),
        status = a.Status.ToString(), a.DataHoraAgendada, a.DuracaoMinutos,
        a.IniciadoEm, a.FinalizadoEm, a.Motivo, a.Observacoes, a.MotivoCancelamento, a.CriadoEm
    };

    private static string CorPorTipo(TipoAgendamento tipo) => tipo switch
    {
        TipoAgendamento.Consulta => "#3b82f6", TipoAgendamento.Retorno => "#22c55e",
        TipoAgendamento.Cirurgia => "#ef4444", TipoAgendamento.Exame => "#f97316",
        TipoAgendamento.Vacinacao => "#a855f7", TipoAgendamento.BanhoTosa => "#06b6d4",
        TipoAgendamento.Emergencia => "#dc2626", _ => "#6b7280"
    };
}

public record CriarAgendamentoDto(Guid PacienteId, string Tipo, DateTime DataHoraAgendada, int DuracaoMinutos = 30, Guid? VeterinarioId = null, string? Motivo = null, string? Observacoes = null);
public record CancelarAgendamentoDto(string? MotivoCancelamento = null);
