namespace PetCore.Domain.Entities;

public class UnidadeProduto
{
    public Guid Id { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }

    public ICollection<Produto> Produtos { get; set; } = [];
}
