namespace PetCore.Domain.Entities;

public class TipoExame
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public decimal? PrecoDefault { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }
}
