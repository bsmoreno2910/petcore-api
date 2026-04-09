using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Comum;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/tutores")]
[Authorize(Roles = "SuperAdmin,Admin,Veterinario,Recepcionista")]
public class TutoresController : ControllerBase
{
    private readonly ServicoTutor _servico;
    public TutoresController(ServicoTutor servico) { _servico = servico; }

    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20, [FromQuery] string? busca = null, [FromQuery] string? telefone = null, [FromQuery] string? cpf = null)
    {
        var (itens, total) = await _servico.ListarPaginadoAsync(ClinicaId, pagina, tamanhoPagina, busca, telefone, cpf);
        return Ok(new RespostaPaginada<object>
        {
            Itens = itens.Select(t => new { t.Id, t.ClinicaId, t.Nome, t.Cpf, t.Rg, t.Telefone, t.TelefoneSecundario, t.Email, t.Rua, t.Numero, t.Complemento, t.Bairro, t.Cidade, t.Estado, t.Cep, t.Observacoes, t.Ativo, t.CriadoEm, quantidadePacientes = t.Pacientes.Count } as object).ToList(),
            TotalRegistros = total, Pagina = pagina, TamanhoPagina = tamanhoPagina
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var t = await _servico.ObterPorIdAsync(id, ClinicaId);
        if (t == null) return NotFound();
        return Ok(new
        {
            t.Id, t.ClinicaId, t.Nome, t.Cpf, t.Rg, t.Telefone, t.TelefoneSecundario, t.Email,
            t.Rua, t.Numero, t.Complemento, t.Bairro, t.Cidade, t.Estado, t.Cep,
            t.Observacoes, t.Ativo, t.CriadoEm, quantidadePacientes = t.Pacientes.Count,
            pacientes = t.Pacientes.Select(p => new { p.Id, p.Nome, nomeEspecie = p.Especie.Nome, nomeRaca = p.Raca?.Nome, p.Ativo })
        });
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarTutorDto dto)
    {
        var tutor = new Tutor { Nome = dto.Nome, Cpf = dto.Cpf, Rg = dto.Rg, Telefone = dto.Telefone, TelefoneSecundario = dto.TelefoneSecundario, Email = dto.Email, Rua = dto.Rua, Numero = dto.Numero, Complemento = dto.Complemento, Bairro = dto.Bairro, Cidade = dto.Cidade, Estado = dto.Estado, Cep = dto.Cep, Observacoes = dto.Observacoes };
        var criado = await _servico.CriarAsync(tutor, ClinicaId);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, new { criado.Id, criado.Nome });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] CriarTutorDto dto)
    {
        var t = await _servico.AtualizarAsync(id, ClinicaId, x => { x.Nome = dto.Nome; x.Cpf = dto.Cpf; x.Rg = dto.Rg; x.Telefone = dto.Telefone; x.TelefoneSecundario = dto.TelefoneSecundario; x.Email = dto.Email; x.Rua = dto.Rua; x.Numero = dto.Numero; x.Complemento = dto.Complemento; x.Bairro = dto.Bairro; x.Cidade = dto.Cidade; x.Estado = dto.Estado; x.Cep = dto.Cep; x.Observacoes = dto.Observacoes; });
        return t == null ? NotFound() : Ok(new { t.Id, t.Nome });
    }

    [HttpGet("{id:guid}/pacientes")]
    public async Task<IActionResult> ListarPacientes(Guid id) =>
        Ok(await _servico.ListarPacientesAsync(id, ClinicaId));

    [HttpGet("{id:guid}/resumo-financeiro")]
    public async Task<IActionResult> ResumoFinanceiro(Guid id)
    {
        var (receita, pago, pendente, atrasado) = await _servico.ObterResumoFinanceiroAsync(id, ClinicaId);
        return Ok(new { totalReceita = receita, totalPago = pago, totalPendente = pendente, totalAtrasado = atrasado });
    }
}

public record CriarTutorDto(string Nome, string? Cpf = null, string? Rg = null, string? Telefone = null, string? TelefoneSecundario = null, string? Email = null, string? Rua = null, string? Numero = null, string? Complemento = null, string? Bairro = null, string? Cidade = null, string? Estado = null, string? Cep = null, string? Observacoes = null);
