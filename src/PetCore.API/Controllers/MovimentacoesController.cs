using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Comum;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/movimentacoes")]
[Authorize]
public class MovimentacoesController : ControllerBase
{
    private readonly ServicoMovimentacao _servico;
    public MovimentacoesController(ServicoMovimentacao servico) { _servico = servico; }
    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;
    private Guid UsuarioId => (Guid)HttpContext.Items["UsuarioId"]!;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20, [FromQuery] TipoMovimentacao? tipo = null, [FromQuery] Guid? produtoId = null, [FromQuery] Guid? usuarioId = null, [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var (itens, total) = await _servico.ListarPaginadoAsync(ClinicaId, pagina, tamanhoPagina, tipo, produtoId, usuarioId, dataInicio, dataFim);
        return Ok(new RespostaPaginada<object>
        {
            Itens = itens.Select(m => new { m.Id, m.ProdutoId, nomeProduto = m.Produto.Nome, tipo = m.Tipo.ToString(), m.Quantidade, m.EstoqueAnterior, m.NovoEstoque, m.Motivo, m.Observacoes, m.CriadoPorId, nomeCriadoPor = m.CriadoPor.Nome, m.CriadoEm } as object).ToList(),
            TotalRegistros = total, Pagina = pagina, TamanhoPagina = tamanhoPagina
        });
    }

    [HttpPost("entrada")]
    [Authorize(Roles = "SuperAdmin,Admin,Operador")]
    public Task<IActionResult> Entrada([FromBody] CriarMovimentacaoDto dto) => CriarMov(dto, _servico.CriarEntradaAsync);

    [HttpPost("saida")]
    [Authorize(Roles = "SuperAdmin,Admin,Operador")]
    public Task<IActionResult> Saida([FromBody] CriarMovimentacaoDto dto) => CriarMov(dto, _servico.CriarSaidaAsync);

    [HttpPost("ajuste")]
    [Authorize(Roles = "SuperAdmin,Admin,Operador")]
    public Task<IActionResult> Ajuste([FromBody] CriarMovimentacaoDto dto) => CriarMov(dto, _servico.CriarAjusteAsync);

    [HttpPost("perda")]
    [Authorize(Roles = "SuperAdmin,Admin,Operador")]
    public Task<IActionResult> Perda([FromBody] CriarMovimentacaoDto dto) => CriarMov(dto, _servico.CriarPerdaAsync);

    private async Task<IActionResult> CriarMov(CriarMovimentacaoDto dto, Func<Guid, Guid, Guid, int, string?, string?, Task<Domain.Entities.Movimentacao>> factory)
    {
        try { var m = await factory(ClinicaId, UsuarioId, dto.ProdutoId, dto.Quantidade, dto.Motivo, dto.Observacoes); return Created("", m); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }
}

public record CriarMovimentacaoDto(Guid ProdutoId, int Quantidade, string? Motivo = null, string? Observacoes = null);
