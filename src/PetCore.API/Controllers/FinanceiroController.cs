using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Comum;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/financeiro")]
[Authorize]
public class FinanceiroController : ControllerBase
{
    private readonly ServicoFinanceiro _servico;
    public FinanceiroController(ServicoFinanceiro servico) { _servico = servico; }
    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;
    private Guid UsuarioId => (Guid)HttpContext.Items["UsuarioId"]!;

    [HttpGet("categorias")]
    public async Task<IActionResult> ListarCategorias() => Ok(await _servico.ListarCategoriasAsync());

    [HttpPost("categorias")]
    public async Task<IActionResult> CriarCategoria([FromBody] CriarCategoriaFinanceiraDto dto)
    {
        if (!Enum.TryParse<TipoTransacao>(dto.Tipo, true, out var tipo)) return BadRequest(new { erro = "Tipo inválido." });
        return Created("", await _servico.CriarCategoriaAsync(dto.Nome, tipo));
    }

    [HttpGet("transacoes")]
    public async Task<IActionResult> ListarTransacoes([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20, [FromQuery] TipoTransacao? tipo = null, [FromQuery] StatusTransacao? status = null, [FromQuery] Guid? categoriaId = null, [FromQuery] Guid? tutorId = null, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var (itens, total) = await _servico.ListarPaginadoAsync(ClinicaId, pagina, tamanhoPagina, tipo, status, categoriaId, tutorId, dataInicio, dataFim);
        return Ok(new RespostaPaginada<object>
        {
            Itens = itens.Select(t => new { t.Id, tipo = t.Tipo.ToString(), status = t.Status.ToString(), t.CategoriaFinanceiraId, nomeCategoriaFinanceira = t.CategoriaFinanceira.Nome, t.Descricao, t.Valor, t.Desconto, t.ValorPago, metodoPagamento = t.MetodoPagamento?.ToString(), t.DataVencimento, t.DataPagamento, t.TutorId, nomeTutor = t.Tutor?.Nome, t.CentroCustoId, nomeCentroCusto = t.CentroCusto?.Nome, t.Observacoes, t.NumeroNota, t.CriadoPorId, nomeCriadoPor = t.CriadoPor.Nome, t.CriadoEm, parcelas = t.Parcelas.Select(p => new { p.Id, p.NumeroParcela, p.Valor, p.DataVencimento, p.DataPagamento, status = p.Status.ToString() }) } as object).ToList(),
            TotalRegistros = total, Pagina = pagina, TamanhoPagina = tamanhoPagina
        });
    }

    [HttpGet("transacoes/{id:guid}")]
    public async Task<IActionResult> ObterTransacao(Guid id)
    {
        var t = await _servico.ObterPorIdAsync(id, ClinicaId);
        return t == null ? NotFound() : Ok(t);
    }

    [HttpPost("transacoes")]
    public async Task<IActionResult> CriarTransacao([FromBody] CriarTransacaoDto dto)
    {
        if (!Enum.TryParse<TipoTransacao>(dto.Tipo, true, out var tipo)) return BadRequest(new { erro = "Tipo inválido." });
        Enum.TryParse<MetodoPagamento>(dto.MetodoPagamento, true, out var metodo);
        var t = await _servico.CriarAsync(ClinicaId, UsuarioId, tipo, dto.CategoriaFinanceiraId, dto.Descricao, dto.Valor, dto.Desconto, string.IsNullOrEmpty(dto.MetodoPagamento) ? null : metodo, dto.DataVencimento, dto.TutorId, dto.AgendamentoId, dto.InternacaoId, dto.SolicitacaoExameId, dto.CentroCustoId, dto.Observacoes, dto.NumeroNota, dto.Parcelas);
        return Created("", t);
    }

    [HttpPatch("transacoes/{id:guid}/pagar")]
    public async Task<IActionResult> Pagar(Guid id, [FromBody] PagarTransacaoDto dto)
    {
        Enum.TryParse<MetodoPagamento>(dto.MetodoPagamento, true, out var metodo);
        var t = await _servico.PagarAsync(id, ClinicaId, dto.ValorPago, string.IsNullOrEmpty(dto.MetodoPagamento) ? null : metodo);
        return t == null ? NotFound() : Ok(t);
    }

    [HttpPatch("transacoes/{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        var t = await _servico.CancelarAsync(id, ClinicaId);
        return t == null ? NotFound() : Ok(t);
    }

    [HttpGet("resumo")]
    public async Task<IActionResult> Resumo()
    {
        var (rec, desp, pend, atrasado, total) = await _servico.ObterResumoAsync(ClinicaId);
        return Ok(new { totalReceita = rec, totalDespesa = desp, saldo = rec - desp, totalPendente = pend, totalAtrasado = atrasado, totalTransacoes = total });
    }

    [HttpGet("vencidas")]
    public async Task<IActionResult> Vencidas() => Ok(await _servico.ListarAtrasadasAsync(ClinicaId));

    [HttpPatch("parcelas/{id:guid}/pagar")]
    public async Task<IActionResult> PagarParcela(Guid id)
    {
        var p = await _servico.PagarParcelaAsync(id);
        return p == null ? NotFound() : Ok(p);
    }
}

public record CriarCategoriaFinanceiraDto(string Nome, string Tipo);
public record CriarTransacaoDto(string Tipo, Guid CategoriaFinanceiraId, string Descricao, decimal Valor, DateTime DataVencimento, decimal? Desconto = null, string? MetodoPagamento = null, Guid? TutorId = null, Guid? AgendamentoId = null, Guid? InternacaoId = null, Guid? SolicitacaoExameId = null, Guid? CentroCustoId = null, string? Observacoes = null, string? NumeroNota = null, int? Parcelas = null);
public record PagarTransacaoDto(decimal? ValorPago = null, string? MetodoPagamento = null);
