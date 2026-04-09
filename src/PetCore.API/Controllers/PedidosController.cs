using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/pedidos")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly ServicoPedido _servico;
    public PedidosController(ServicoPedido servico) { _servico = servico; }
    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;
    private Guid UsuarioId => (Guid)HttpContext.Items["UsuarioId"]!;

    [HttpGet]
    public async Task<IActionResult> ListarTodos() => Ok(await _servico.ListarTodosAsync(ClinicaId));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var p = await _servico.ObterPorIdAsync(id, ClinicaId);
        return p == null ? NotFound() : Ok(p);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Operador")]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoDto dto)
    {
        var tipo = Enum.TryParse<TipoPedido>(dto.Tipo, true, out var t) ? t : TipoPedido.Regular;
        var itens = dto.Itens.Select(i => (i.ProdutoId, i.QuantidadeSolicitada, i.Observacoes)).ToList();
        var pedido = await _servico.CriarAsync(ClinicaId, UsuarioId, tipo, dto.Periodo, dto.Observacoes, dto.Justificativa, itens);
        return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
    }

    [HttpPatch("{id:guid}/enviar")]
    public async Task<IActionResult> Enviar(Guid id)
    {
        try { var p = await _servico.EnviarAsync(id, ClinicaId); return p == null ? NotFound() : Ok(p); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpPatch("{id:guid}/aprovar")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Aprovar(Guid id)
    {
        try { var p = await _servico.AprovarAsync(id, ClinicaId, UsuarioId); return p == null ? NotFound() : Ok(p); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpPatch("{id:guid}/receber")]
    public async Task<IActionResult> Receber(Guid id, [FromBody] ReceberPedidoDto dto)
    {
        try
        {
            var itens = dto.Itens.Select(i => (i.ItemId, i.QuantidadeRecebida)).ToList();
            var p = await _servico.ReceberAsync(id, ClinicaId, UsuarioId, itens);
            return p == null ? NotFound() : Ok(p);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpPatch("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        try { var p = await _servico.CancelarAsync(id, ClinicaId); return p == null ? NotFound() : Ok(p); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }
}

public record CriarPedidoDto(string? Tipo = null, string? Periodo = null, string? Observacoes = null, string? Justificativa = null, List<ItemPedidoDto>? Itens = null)
{
    public List<ItemPedidoDto> Itens { get; init; } = Itens ?? [];
}
public record ItemPedidoDto(Guid ProdutoId, int QuantidadeSolicitada, string? Observacoes = null);
public record ReceberPedidoDto(List<ItemRecebidoDto> Itens);
public record ItemRecebidoDto(Guid ItemId, int QuantidadeRecebida);
