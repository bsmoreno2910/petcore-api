namespace PetCore.Domain.Entities;

public class TokenAtualizacao
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? RevogadoEm { get; set; }
    public string? SubstituidoPor { get; set; }

    public bool Expirado => DateTime.UtcNow >= ExpiraEm;
    public bool Ativo => RevogadoEm == null && !Expirado;

    public Usuario Usuario { get; set; } = null!;
}
