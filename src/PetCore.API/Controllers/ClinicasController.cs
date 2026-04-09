using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/clinicas")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class ClinicasController : ControllerBase
{
    private readonly ServicoClinica _servico;
    public ClinicasController(ServicoClinica servico) { _servico = servico; }

    [HttpGet]
    public async Task<IActionResult> ListarTodas() => Ok(await _servico.ListarTodasAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var c = await _servico.ObterPorIdAsync(id);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Criar([FromBody] CriarClinicaDto dto)
    {
        var c = await _servico.CriarAsync(new() { Nome = dto.Nome, NomeFantasia = dto.NomeFantasia, RazaoSocial = dto.RazaoSocial, Cnpj = dto.Cnpj, Telefone = dto.Telefone, Email = dto.Email, Website = dto.Website, Rua = dto.Rua, Numero = dto.Numero, Complemento = dto.Complemento, Bairro = dto.Bairro, Cidade = dto.Cidade, Estado = dto.Estado, Cep = dto.Cep });
        return CreatedAtAction(nameof(ObterPorId), new { id = c.Id }, c);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] CriarClinicaDto dto)
    {
        var c = await _servico.AtualizarAsync(id, x => { x.Nome = dto.Nome; x.NomeFantasia = dto.NomeFantasia; x.Cnpj = dto.Cnpj; x.Telefone = dto.Telefone; x.Email = dto.Email; x.Cidade = dto.Cidade; x.Estado = dto.Estado; });
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPatch("{id:guid}/alternar-status")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AlternarStatus(Guid id) =>
        await _servico.AlternarAtivoAsync(id) ? Ok(new { mensagem = "Status alterado." }) : NotFound();

    [HttpGet("{id:guid}/usuarios")]
    public async Task<IActionResult> ListarUsuarios(Guid id)
    {
        var usuarios = await _servico.ListarUsuariosAsync(id);
        return Ok(usuarios.Select(cu => new { cu.Id, cu.UsuarioId, nomeUsuario = cu.Usuario.Nome, emailUsuario = cu.Usuario.Email, perfil = cu.Perfil.ToString(), cu.Ativo }));
    }

    [HttpPost("{id:guid}/usuarios")]
    public async Task<IActionResult> AdicionarUsuario(Guid id, [FromBody] AdicionarUsuarioClinicaDto dto)
    {
        if (!Enum.TryParse<PerfilUsuario>(dto.Perfil, true, out var perfil))
            return BadRequest(new { erro = "Perfil inválido." });
        try
        {
            var cu = await _servico.AdicionarUsuarioAsync(id, dto.UsuarioId, perfil);
            return Created("", new { cu.Id, cu.UsuarioId, nomeUsuario = cu.Usuario.Nome, perfil = cu.Perfil.ToString() });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpDelete("{id:guid}/usuarios/{usuarioId:guid}")]
    public async Task<IActionResult> RemoverUsuario(Guid id, Guid usuarioId) =>
        await _servico.RemoverUsuarioAsync(id, usuarioId) ? NoContent() : NotFound();
}

public record CriarClinicaDto(string Nome, string? NomeFantasia = null, string? RazaoSocial = null, string? Cnpj = null, string? Telefone = null, string? Email = null, string? Website = null, string? Rua = null, string? Numero = null, string? Complemento = null, string? Bairro = null, string? Cidade = null, string? Estado = null, string? Cep = null);
public record AdicionarUsuarioClinicaDto(Guid UsuarioId, string Perfil);
