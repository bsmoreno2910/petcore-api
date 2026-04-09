namespace PetCore.Domain.Entities;

public class ResultadoExame
{
    public Guid Id { get; set; }
    public Guid SolicitacaoExameId { get; set; }
    public Guid RealizadoPorId { get; set; }

    public string? TextoResultado { get; set; }
    public string? ArquivoResultadoUrl { get; set; }
    public string? ValoresReferencia { get; set; }
    public string? Observacoes { get; set; }
    public string? Conclusao { get; set; }

    public DateTime CriadoEm { get; set; }

    public SolicitacaoExame SolicitacaoExame { get; set; } = null!;
    public Usuario RealizadoPor { get; set; } = null!;
}
