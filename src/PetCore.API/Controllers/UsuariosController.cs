using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class UsuariosController : ControllerBase
{
    private readonly ServicoUsuario _servico;
    public UsuariosController(ServicoUsuario servico) { _servico = servico; }

    [HttpGet]
    public async Task<IActionResult> ListarTodos()
    {
        var usuarios = await _servico.ListarTodosAsync();
        return Ok(usuarios.Select(u => new
        {
            u.Id, u.Nome, u.Email, u.Telefone, u.Crmv, u.AvatarUrl, u.Ativo, u.CriadoEm,
            clinicas = u.ClinicaUsuarios.Select(cu => new { clinicaId = cu.ClinicaId, nomeClinica = cu.Clinica.Nome, perfil = cu.Perfil.ToString() })
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var u = await _servico.ObterPorIdAsync(id);
        if (u == null) return NotFound();
        return Ok(new
        {
            u.Id, u.Nome, u.Email, u.Telefone, u.Crmv, u.AvatarUrl, u.Ativo, u.CriadoEm,
            clinicas = u.ClinicaUsuarios.Select(cu => new { clinicaId = cu.ClinicaId, nomeClinica = cu.Clinica.Nome, perfil = cu.Perfil.ToString() })
        });
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarUsuarioDto dto)
    {
        try
        {
            var u = await _servico.CriarAsync(dto.Nome, dto.Email, dto.Senha, dto.Telefone, dto.Crmv);
            return CreatedAtAction(nameof(ObterPorId), new { id = u.Id }, new { u.Id, u.Nome, u.Email });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarUsuarioDto dto)
    {
        var u = await _servico.AtualizarAsync(id, x => { x.Nome = dto.Nome; if (dto.Telefone != null) x.Telefone = dto.Telefone; if (dto.Crmv != null) x.Crmv = dto.Crmv; });
        return u == null ? NotFound() : Ok(new { u.Id, u.Nome, u.Email });
    }

    [HttpPatch("{id:guid}/alternar-status")]
    public async Task<IActionResult> AlternarStatus(Guid id) =>
        await _servico.AlternarAtivoAsync(id) ? Ok(new { mensagem = "Status alterado." }) : NotFound();
}

public record CriarUsuarioDto(string Nome, string Email, string Senha, string? Telefone = null, string? Crmv = null);
public record AtualizarUsuarioDto(string Nome, string? Telefone = null, string? Crmv = null, string? AvatarUrl = null);
