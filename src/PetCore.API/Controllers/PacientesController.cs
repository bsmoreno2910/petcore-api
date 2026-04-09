using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Comum;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/pacientes")]
[Authorize(Roles = "SuperAdmin,Admin,Veterinario,Recepcionista")]
public class PacientesController : ControllerBase
{
    private readonly ServicoPaciente _servico;
    public PacientesController(ServicoPaciente servico) { _servico = servico; }

    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20, [FromQuery] string? busca = null, [FromQuery] Guid? especieId = null, [FromQuery] Guid? tutorId = null)
    {
        var (itens, total) = await _servico.ListarPaginadoAsync(ClinicaId, pagina, tamanhoPagina, busca, especieId, tutorId);
        return Ok(new RespostaPaginada<object>
        {
            Itens = itens.Select(p => new { p.Id, p.ClinicaId, p.TutorId, nomeTutor = p.Tutor.Nome, p.EspecieId, nomeEspecie = p.Especie.Nome, p.RacaId, nomeRaca = p.Raca?.Nome, p.Nome, sexo = p.Sexo.ToString(), p.DataNascimento, p.Peso, p.Cor, p.Microchip, p.Castrado, p.Alergias, p.Observacoes, p.FotoUrl, p.Ativo, p.Obito, p.CriadoEm } as object).ToList(),
            TotalRegistros = total, Pagina = pagina, TamanhoPagina = tamanhoPagina
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var p = await _servico.ObterPorIdAsync(id, ClinicaId);
        if (p == null) return NotFound();
        return Ok(new { p.Id, p.ClinicaId, p.TutorId, nomeTutor = p.Tutor.Nome, telefoneTutor = p.Tutor.Telefone, emailTutor = p.Tutor.Email, p.EspecieId, nomeEspecie = p.Especie.Nome, p.RacaId, nomeRaca = p.Raca?.Nome, p.Nome, sexo = p.Sexo.ToString(), p.DataNascimento, p.Peso, p.Cor, p.Microchip, p.Castrado, p.Alergias, p.Observacoes, p.FotoUrl, p.Ativo, p.Obito, p.DataObito, p.CriadoEm });
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPacienteDto dto)
    {
        var paciente = new Paciente
        {
            TutorId = dto.TutorId, EspecieId = dto.EspecieId, RacaId = dto.RacaId, Nome = dto.Nome,
            Sexo = Enum.TryParse<SexoPaciente>(dto.Sexo, true, out var s) ? s : SexoPaciente.Desconhecido,
            DataNascimento = dto.DataNascimento, Peso = dto.Peso, Cor = dto.Cor, Microchip = dto.Microchip,
            Castrado = dto.Castrado, Alergias = dto.Alergias, Observacoes = dto.Observacoes, FotoUrl = dto.FotoUrl
        };
        var criado = await _servico.CriarAsync(paciente, ClinicaId);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, new { criado.Id, criado.Nome });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] CriarPacienteDto dto)
    {
        var p = await _servico.AtualizarAsync(id, ClinicaId, x =>
        {
            x.TutorId = dto.TutorId; x.EspecieId = dto.EspecieId; x.RacaId = dto.RacaId; x.Nome = dto.Nome;
            x.Sexo = Enum.TryParse<SexoPaciente>(dto.Sexo, true, out var s) ? s : SexoPaciente.Desconhecido;
            x.DataNascimento = dto.DataNascimento; x.Peso = dto.Peso; x.Cor = dto.Cor; x.Microchip = dto.Microchip;
            x.Castrado = dto.Castrado; x.Alergias = dto.Alergias; x.Observacoes = dto.Observacoes;
        });
        return p == null ? NotFound() : Ok(new { p.Id, p.Nome });
    }

    [HttpGet("{id:guid}/prontuarios")]
    public async Task<IActionResult> ListarProntuarios(Guid id)
    {
        var registros = await _servico.ListarProntuariosAsync(id, ClinicaId);
        return Ok(registros.Select(r => new { r.Id, r.VeterinarioId, nomeVeterinario = r.Veterinario.Nome, r.QueixaPrincipal, r.Diagnostico, r.Tratamento, r.CriadoEm, quantidadePrescricoes = r.Prescricoes.Count }));
    }

    [HttpGet("{id:guid}/exames")]
    public async Task<IActionResult> ListarExames(Guid id)
    {
        var exames = await _servico.ListarExamesAsync(id, ClinicaId);
        return Ok(exames.Select(e => new { e.Id, nomeTipoExame = e.TipoExame.Nome, nomeSolicitante = e.SolicitadoPor.Nome, status = e.Status.ToString(), e.DataSolicitacao, e.DataConclusao, temResultado = e.Resultado != null }));
    }

    [HttpGet("{id:guid}/internacoes")]
    public async Task<IActionResult> ListarInternacoes(Guid id)
    {
        var internacoes = await _servico.ListarInternacoesAsync(id, ClinicaId);
        return Ok(internacoes.Select(h => new { h.Id, nomeVeterinario = h.Veterinario.Nome, status = h.Status.ToString(), h.Motivo, h.DataInternacao, h.DataAlta }));
    }

    [HttpGet("{id:guid}/linha-do-tempo")]
    public async Task<IActionResult> LinhaTempo(Guid id)
    {
        var prontuarios = (await _servico.ListarProntuariosAsync(id, ClinicaId)).Select(r => new { tipo = "prontuario", data = r.CriadoEm, r.Id, descricao = r.QueixaPrincipal ?? r.Diagnostico ?? "Atendimento" });
        var exames = (await _servico.ListarExamesAsync(id, ClinicaId)).Select(e => new { tipo = "exame", data = e.CriadoEm, e.Id, descricao = e.TipoExame.Nome });
        var internacoes = (await _servico.ListarInternacoesAsync(id, ClinicaId)).Select(h => new { tipo = "internacao", data = h.CriadoEm, h.Id, descricao = h.Motivo ?? "Internação" });
        return Ok(prontuarios.Concat(exames).Concat(internacoes).OrderByDescending(t => t.data));
    }
}

public record CriarPacienteDto(Guid TutorId, Guid EspecieId, string Nome, Guid? RacaId = null, string? Sexo = null, DateTime? DataNascimento = null, decimal? Peso = null, string? Cor = null, string? Microchip = null, bool Castrado = false, string? Alergias = null, string? Observacoes = null, string? FotoUrl = null);
