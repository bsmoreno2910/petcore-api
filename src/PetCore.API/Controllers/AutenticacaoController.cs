using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.Domain.Interfaces;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/autenticacao")]
public class AutenticacaoController : ControllerBase
{
    private readonly IServicoAutenticacao _servico;

    public AutenticacaoController(IServicoAutenticacao servico)
    {
        _servico = servico;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var resultado = await _servico.LoginAsync(dto.Email, dto.Senha);
        if (!resultado.Sucesso)
            return Unauthorized(new { erro = resultado.Erro });

        return Ok(new
        {
            tokenAcesso = resultado.TokenAcesso,
            tokenAtualizacao = resultado.TokenAtualizacao,
            usuario = resultado.Usuario,
            clinicas = resultado.Clinicas
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        var resultado = await _servico.RefreshTokenAsync(dto.TokenAtualizacao);
        if (!resultado.Sucesso)
            return Unauthorized(new { erro = resultado.Erro });

        return Ok(new { tokenAcesso = resultado.TokenAcesso, tokenAtualizacao = resultado.TokenAtualizacao });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout() => Ok(new { mensagem = "Logout realizado com sucesso." });

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new
        {
            usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(ClaimTypes.Email),
            nome = User.FindFirstValue(ClaimTypes.Name),
            perfil = User.FindFirstValue(ClaimTypes.Role),
            clinicaId = User.FindFirstValue("clinicaId"),
        });
    }

    [HttpPatch("alterar-senha")]
    [Authorize]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaDto dto)
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            await _servico.AlterarSenhaAsync(usuarioId, dto.SenhaAtual, dto.NovaSenha);
            return Ok(new { mensagem = "Senha alterada com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpPost("selecionar-clinica")]
    [Authorize]
    public async Task<IActionResult> SelecionarClinica([FromBody] SelecionarClinicaDto dto)
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _servico.SelecionarClinicaAsync(usuarioId, dto.ClinicaId);
        if (!resultado.Sucesso)
            return BadRequest(new { erro = resultado.Erro });

        return Ok(new { tokenAcesso = resultado.TokenAcesso, tokenAtualizacao = resultado.TokenAtualizacao });
    }
}

public record LoginDto(string Email, string Senha);
public record RefreshDto(string TokenAtualizacao);
public record AlterarSenhaDto(string SenhaAtual, string NovaSenha);
public record SelecionarClinicaDto(Guid ClinicaId);
