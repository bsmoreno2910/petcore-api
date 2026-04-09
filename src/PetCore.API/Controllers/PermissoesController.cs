using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/permissoes")]
[Authorize]
public class PermissoesController : ControllerBase
{
    private readonly AppDbContext _db;

    private static readonly string[] Modulos =
    [
        "Tutores", "Pacientes", "Agenda", "Prontuarios", "Internacoes",
        "Exames", "Produtos", "Movimentacoes", "Pedidos", "Financeiro",
        "CentrosCusto", "Relatorios", "Usuarios", "Clinicas", "Auditoria"
    ];

    public PermissoesController(AppDbContext db) { _db = db; }

    private string? PerfilAtual => HttpContext.Items["Perfil"] as string;

    /// <summary>
    /// Lista permissões de um perfil específico. Apenas SuperAdmin e Admin.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> ListarPorPerfil([FromQuery] string perfil)
    {
        if (string.IsNullOrWhiteSpace(perfil))
            return BadRequest(new { erro = "O parâmetro 'perfil' é obrigatório." });

        var existentes = await _db.Permissoes
            .Where(p => p.Perfil == perfil)
            .ToListAsync();

        var resultado = Modulos.Select(modulo =>
        {
            var perm = existentes.FirstOrDefault(p => p.Modulo == modulo);
            if (perm != null)
            {
                return new
                {
                    perm.Modulo,
                    perm.PodeVisualizar,
                    perm.PodeAdicionar,
                    perm.PodeEditar,
                    perm.PodeExcluir
                };
            }

            // Default: SuperAdmin tem tudo, outros também começam com tudo true
            var padrao = perfil == "SuperAdmin";
            return new
            {
                Modulo = modulo,
                PodeVisualizar = true,
                PodeAdicionar = padrao || true,
                PodeEditar = padrao || true,
                PodeExcluir = padrao || true
            };
        });

        return Ok(resultado);
    }

    /// <summary>
    /// Atualiza permissões em lote para um perfil. Apenas SuperAdmin e Admin.
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Atualizar([FromBody] AtualizarPermissoesDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Perfil))
            return BadRequest(new { erro = "Perfil é obrigatório." });

        if (dto.Perfil == "SuperAdmin")
            return BadRequest(new { erro = "Não é possível alterar permissões do SuperAdmin." });

        var existentes = await _db.Permissoes
            .Where(p => p.Perfil == dto.Perfil)
            .ToListAsync();

        foreach (var item in dto.Permissoes)
        {
            if (!Modulos.Contains(item.Modulo)) continue;

            var perm = existentes.FirstOrDefault(p => p.Modulo == item.Modulo);
            if (perm != null)
            {
                perm.PodeVisualizar = item.PodeVisualizar;
                perm.PodeAdicionar = item.PodeAdicionar;
                perm.PodeEditar = item.PodeEditar;
                perm.PodeExcluir = item.PodeExcluir;
                perm.AtualizadoEm = DateTime.UtcNow;
            }
            else
            {
                _db.Permissoes.Add(new Permissao
                {
                    Id = Guid.NewGuid(),
                    Perfil = dto.Perfil,
                    Modulo = item.Modulo,
                    PodeVisualizar = item.PodeVisualizar,
                    PodeAdicionar = item.PodeAdicionar,
                    PodeEditar = item.PodeEditar,
                    PodeExcluir = item.PodeExcluir
                });
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { mensagem = "Permissões atualizadas com sucesso." });
    }

    /// <summary>
    /// Retorna as permissões do usuário autenticado com base no seu perfil.
    /// </summary>
    [HttpGet("usuario")]
    public async Task<IActionResult> MinhasPermissoes()
    {
        var perfil = PerfilAtual;
        if (string.IsNullOrEmpty(perfil))
            return Unauthorized(new { erro = "Perfil não encontrado." });

        // SuperAdmin tem tudo
        if (perfil == "SuperAdmin")
        {
            return Ok(Modulos.Select(m => new
            {
                Modulo = m,
                PodeVisualizar = true,
                PodeAdicionar = true,
                PodeEditar = true,
                PodeExcluir = true
            }));
        }

        var existentes = await _db.Permissoes
            .Where(p => p.Perfil == perfil)
            .ToListAsync();

        var resultado = Modulos.Select(modulo =>
        {
            var perm = existentes.FirstOrDefault(p => p.Modulo == modulo);
            return new
            {
                Modulo = modulo,
                PodeVisualizar = perm?.PodeVisualizar ?? true,
                PodeAdicionar = perm?.PodeAdicionar ?? true,
                PodeEditar = perm?.PodeEditar ?? true,
                PodeExcluir = perm?.PodeExcluir ?? true
            };
        });

        return Ok(resultado);
    }
}

public record PermissaoItemDto(string Modulo, bool PodeVisualizar, bool PodeAdicionar, bool PodeEditar, bool PodeExcluir);
public record AtualizarPermissoesDto(string Perfil, List<PermissaoItemDto> Permissoes);
