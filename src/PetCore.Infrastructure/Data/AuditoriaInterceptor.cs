using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PetCore.Domain.Entities;

namespace PetCore.Infrastructure.Data;

public class AuditoriaInterceptor : SaveChangesInterceptor
{
    private readonly ITenantProvider _tenant;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AuditoriaInterceptor(ITenantProvider tenant, IHttpContextAccessor? httpContextAccessor = null)
    {
        _tenant = tenant;
        _httpContextAccessor = httpContextAccessor;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var db = eventData.Context;
        if (db == null || _tenant.UsuarioId == null || _tenant.ClinicaId == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = db.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .Where(e => e.Entity is not LogAuditoria && e.Entity is not TokenAtualizacao)
            .ToList();

        foreach (var entry in entries)
        {
            var entityName = entry.Entity.GetType().Name;
            var entityId = entry.Property("Id").CurrentValue?.ToString() ?? "";
            var acao = entry.State switch
            {
                EntityState.Added => "Criar",
                EntityState.Modified => "Editar",
                EntityState.Deleted => "Excluir",
                _ => "Desconhecido"
            };

            string? valorAntigo = null;
            string? novoValor = null;

            if (entry.State == EntityState.Modified)
            {
                var props = entry.Properties.Where(p => p.IsModified).ToList();
                valorAntigo = JsonSerializer.Serialize(props.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue?.ToString()));
                novoValor = JsonSerializer.Serialize(props.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString()));
            }
            else if (entry.State == EntityState.Added)
            {
                novoValor = JsonSerializer.Serialize(entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString()));
            }

            var ip = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            db.Set<LogAuditoria>().Add(new LogAuditoria
            {
                Id = Guid.NewGuid(),
                ClinicaId = _tenant.ClinicaId.Value,
                UsuarioId = _tenant.UsuarioId.Value,
                Acao = acao,
                Entidade = entityName,
                EntidadeId = entityId,
                ValorAntigo = valorAntigo,
                NovoValor = novoValor,
                EnderecoIp = ip,
                CriadoEm = DateTime.UtcNow
            });
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
