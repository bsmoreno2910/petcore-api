using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/especies")]
[Authorize]
public class EspeciesController : ControllerBase
{
    private readonly ServicoEspecie _servico;
    public EspeciesController(ServicoEspecie servico) { _servico = servico; }

    [HttpGet]
    public async Task<IActionResult> ListarTodas()
    {
        var especies = await _servico.ListarTodasAsync();
        return Ok(especies.Select(s => new { s.Id, s.Nome, s.Ativo, racas = s.Racas.Select(r => new { r.Id, r.EspecieId, r.Nome, r.Ativo }) }));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Criar([FromBody] NomeDto dto)
    {
        try { return Created("", await _servico.CriarAsync(dto.Nome)); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpGet("{id:guid}/racas")]
    public async Task<IActionResult> ListarRacas(Guid id) => Ok(await _servico.ListarRacasAsync(id));

    [HttpPost("{id:guid}/racas")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CriarRaca(Guid id, [FromBody] NomeDto dto)
    {
        try { return Created("", await _servico.CriarRacaAsync(id, dto.Nome)); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }
}

public record NomeDto(string Nome);
