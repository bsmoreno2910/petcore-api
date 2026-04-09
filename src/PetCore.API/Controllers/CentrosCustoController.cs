using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/centros-custo")]
[Authorize]
public class CentrosCustoController : ControllerBase
{
    private readonly ServicoCentroCusto _servico;
    public CentrosCustoController(ServicoCentroCusto servico) { _servico = servico; }
    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;

    [HttpGet]
    public async Task<IActionResult> ListarTodos() => Ok(await _servico.ListarTodosAsync(ClinicaId));

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarCentroCustoDto dto) =>
        Created("", await _servico.CriarAsync(ClinicaId, dto.Nome, dto.Descricao));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] CriarCentroCustoDto dto)
    {
        var cc = await _servico.AtualizarAsync(id, ClinicaId, dto.Nome, dto.Descricao);
        return cc == null ? NotFound() : Ok(cc);
    }

    [HttpGet("{id:guid}/resumo")]
    public async Task<IActionResult> Resumo(Guid id)
    {
        var (rec, desp, total) = await _servico.ObterResumoAsync(id, ClinicaId);
        return Ok(new { centroCustoId = id, totalReceita = rec, totalDespesa = desp, saldo = rec - desp, totalTransacoes = total });
    }

    [HttpGet("relatorio")]
    public async Task<IActionResult> Relatorio()
    {
        var dados = await _servico.ObterRelatorioAsync(ClinicaId);
        return Ok(dados.Select(d => new { centroCustoId = d.Id, nomeCentroCusto = d.Nome, totalReceita = d.Receita, totalDespesa = d.Despesa, saldo = d.Receita - d.Despesa, totalTransacoes = d.Total }));
    }
}

public record CriarCentroCustoDto(string Nome, string? Descricao = null);
