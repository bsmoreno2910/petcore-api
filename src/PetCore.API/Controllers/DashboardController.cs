using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ServicoDashboard _servico;
    private readonly ServicoProduto _servicoProduto;

    public DashboardController(ServicoDashboard servico, ServicoProduto servicoProduto)
    { _servico = servico; _servicoProduto = servicoProduto; }

    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;

    [HttpGet("resumo")]
    public async Task<IActionResult> Resumo()
    {
        var (pacientes, tutores, agHoje, intAtivas, exPend, estBaixo, recMes, despMes) = await _servico.ObterResumoAsync(ClinicaId);
        return Ok(new
        {
            totalPacientes = pacientes, totalTutores = tutores, agendamentosHoje = agHoje,
            internacoesAtivas = intAtivas, examesPendentes = exPend, produtosEstoqueBaixo = estBaixo,
            receitaMensal = recMes, despesaMensal = despMes
        });
    }

    [HttpGet("receita-despesa-mensal")]
    public async Task<IActionResult> ReceitaDespesaMensal()
    {
        var dados = await _servico.ObterReceitaDespesaMensalAsync(ClinicaId);
        var meses = new[] { "", "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" };
        return Ok(dados.Select(d => new { mes = $"{meses[d.Mes]}/{d.Ano % 100:D2}", receita = d.Receita, despesa = d.Despesa }));
    }

    [HttpGet("agendamentos-por-tipo")]
    public async Task<IActionResult> AgendamentosPorTipo()
    {
        var dados = await _servico.ObterAgendamentosPorTipoAsync(ClinicaId);
        return Ok(dados.Select(d => new { tipo = d.Tipo, quantidade = d.Quantidade }));
    }

    [HttpGet("alertas-estoque")]
    public async Task<IActionResult> AlertasEstoque()
    {
        var zerados = await _servicoProduto.ListarEstoqueZeradoAsync(ClinicaId);
        var baixos = await _servicoProduto.ListarEstoqueBaixoAsync(ClinicaId);
        var vencendo = await _servicoProduto.ListarVencimentoProximoAsync(ClinicaId, 30);

        var alertas = zerados.Select(p => new { p.Id, p.Nome, nomeCategoria = p.Categoria.Nome, p.EstoqueAtual, p.EstoqueMinimo, tipoAlerta = "Zerado", dataValidade = (DateTime?)null })
            .Concat(baixos.Select(p => new { p.Id, p.Nome, nomeCategoria = p.Categoria.Nome, p.EstoqueAtual, p.EstoqueMinimo, tipoAlerta = "Baixo", dataValidade = (DateTime?)null }))
            .Concat(vencendo.Select(p => new { p.Id, p.Nome, nomeCategoria = p.Categoria.Nome, p.EstoqueAtual, p.EstoqueMinimo, tipoAlerta = "Vencendo", dataValidade = p.DataValidade }));
        return Ok(alertas);
    }
}
