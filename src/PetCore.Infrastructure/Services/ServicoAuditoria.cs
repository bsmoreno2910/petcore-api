using PetCore.Domain.Entities;
using PetCore.Domain.Interfaces;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoAuditoria : IServicoAuditoria
{
    private readonly AppDbContext _db;

    public ServicoAuditoria(AppDbContext db)
    {
        _db = db;
    }

    public async Task RegistrarAsync(Guid clinicaId, Guid usuarioId, string acao, string entidade, string entidadeId,
        string? valorAntigo = null, string? novoValor = null, string? enderecoIp = null)
    {
        var log = new LogAuditoria
        {
            Id = Guid.NewGuid(),
            ClinicaId = clinicaId,
            UsuarioId = usuarioId,
            Acao = acao,
            Entidade = entidade,
            EntidadeId = entidadeId,
            ValorAntigo = valorAntigo,
            NovoValor = novoValor,
            EnderecoIp = enderecoIp,
            CriadoEm = DateTime.UtcNow
        };

        _db.LogsAuditoria.Add(log);
        await _db.SaveChangesAsync();
    }
}
