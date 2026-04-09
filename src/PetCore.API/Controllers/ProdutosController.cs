using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Comum;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly ServicoProduto _servico;
    public ProdutosController(ServicoProduto servico) { _servico = servico; }
    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;

    [HttpGet("api/categorias-produto")]
    public async Task<IActionResult> ListarCategorias() => Ok(await _servico.ListarCategoriasAsync());

    [HttpPost("api/categorias-produto")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CriarCategoria([FromBody] CriarCategoriaProdutoDto dto) =>
        Created("", await _servico.CriarCategoriaAsync(dto.Nome, dto.Descricao, dto.Cor));

    [HttpPut("api/categorias-produto/{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> AtualizarCategoria(Guid id, [FromBody] CriarCategoriaProdutoDto dto)
    {
        var c = await _servico.AtualizarCategoriaAsync(id, dto.Nome, dto.Descricao, dto.Cor);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpGet("api/unidades-produto")]
    public async Task<IActionResult> ListarUnidades() => Ok(await _servico.ListarUnidadesAsync());

    [HttpPost("api/unidades-produto")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CriarUnidade([FromBody] CriarUnidadeProdutoDto dto) =>
        Created("", await _servico.CriarUnidadeAsync(dto.Sigla, dto.Nome));

    [HttpGet("api/produtos")]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20, [FromQuery] string? busca = null, [FromQuery] Guid? categoriaId = null, [FromQuery] string? statusEstoque = null)
    {
        var (itens, total) = await _servico.ListarPaginadoAsync(ClinicaId, pagina, tamanhoPagina, busca, categoriaId, statusEstoque);
        return Ok(new RespostaPaginada<object>
        {
            Itens = itens.Select(p => new { p.Id, p.ClinicaId, p.CategoriaId, nomeCategoria = p.Categoria.Nome, corCategoria = p.Categoria.Cor, p.UnidadeId, siglaUnidade = p.Unidade.Sigla, p.Nome, p.Apresentacao, p.EstoqueAtual, p.EstoqueMinimo, p.EstoqueMaximo, p.PrecoCusto, p.PrecoVenda, p.Localizacao, p.CodigoBarras, p.Lote, p.DataValidade, p.Ativo, p.Observacoes, p.CriadoEm, statusEstoque = p.EstoqueAtual <= 0 ? "Zerado" : p.EstoqueAtual <= p.EstoqueMinimo ? "Baixo" : "Ok" } as object).ToList(),
            TotalRegistros = total, Pagina = pagina, TamanhoPagina = tamanhoPagina
        });
    }

    [HttpGet("api/produtos/{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var p = await _servico.ObterPorIdAsync(id, ClinicaId);
        return p == null ? NotFound() : Ok(p);
    }

    [HttpPost("api/produtos")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Criar([FromBody] CriarProdutoDto dto)
    {
        var produto = new Produto { CategoriaId = dto.CategoriaId, UnidadeId = dto.UnidadeId, Nome = dto.Nome, Apresentacao = dto.Apresentacao, EstoqueAtual = dto.EstoqueAtual, EstoqueMinimo = dto.EstoqueMinimo, EstoqueMaximo = dto.EstoqueMaximo, PrecoCusto = dto.PrecoCusto, PrecoVenda = dto.PrecoVenda, Localizacao = dto.Localizacao, CodigoBarras = dto.CodigoBarras, Lote = dto.Lote, DataValidade = dto.DataValidade, Observacoes = dto.Observacoes };
        var criado = await _servico.CriarAsync(produto, ClinicaId);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    [HttpDelete("api/produtos/{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Excluir(Guid id) =>
        await _servico.ExcluirAsync(id, ClinicaId) ? NoContent() : NotFound();

    [HttpGet("api/produtos/estoque-baixo")]
    public async Task<IActionResult> EstoqueBaixo() => Ok(await _servico.ListarEstoqueBaixoAsync(ClinicaId));

    [HttpGet("api/produtos/estoque-zerado")]
    public async Task<IActionResult> EstoqueZerado() => Ok(await _servico.ListarEstoqueZeradoAsync(ClinicaId));

    [HttpGet("api/produtos/vencimento-proximo")]
    public async Task<IActionResult> VencimentoProximo([FromQuery] int dias = 30) => Ok(await _servico.ListarVencimentoProximoAsync(ClinicaId, dias));
}

public record CriarCategoriaProdutoDto(string Nome, string? Descricao = null, string? Cor = null);
public record CriarUnidadeProdutoDto(string Sigla, string Nome);
public record CriarProdutoDto(Guid CategoriaId, Guid UnidadeId, string Nome, string? Apresentacao = null, int EstoqueAtual = 0, int EstoqueMinimo = 0, int? EstoqueMaximo = null, decimal? PrecoCusto = null, decimal? PrecoVenda = null, string? Localizacao = null, string? CodigoBarras = null, string? Lote = null, DateTime? DataValidade = null, string? Observacoes = null);
