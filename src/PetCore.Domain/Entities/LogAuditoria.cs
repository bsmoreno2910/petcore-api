namespace PetCore.Domain.Entities;

public class LogAuditoria
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string Entidade { get; set; } = string.Empty;
    public string EntidadeId { get; set; } = string.Empty;
    public string? ValorAntigo { get; set; }
    public string? NovoValor { get; set; }
    public string? EnderecoIp { get; set; }
    public DateTime CriadoEm { get; set; }

    public Usuario Usuario { get; set; } = null!;
}
