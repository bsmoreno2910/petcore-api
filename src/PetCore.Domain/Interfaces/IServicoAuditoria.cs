namespace PetCore.Domain.Interfaces;

public interface IServicoAuditoria
{
    Task RegistrarAsync(Guid clinicaId, Guid usuarioId, string acao, string entidade, string entidadeId,
        string? valorAntigo = null, string? novoValor = null, string? enderecoIp = null);
}
