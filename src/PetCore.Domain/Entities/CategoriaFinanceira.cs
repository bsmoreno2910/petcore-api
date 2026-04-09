using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class CategoriaFinanceira
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoTransacao Tipo { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }
}
