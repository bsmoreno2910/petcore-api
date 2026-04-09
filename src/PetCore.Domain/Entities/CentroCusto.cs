namespace PetCore.Domain.Entities;

public class CentroCusto
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }

    public Clinica Clinica { get; set; } = null!;
    public ICollection<TransacaoFinanceira> Transacoes { get; set; } = [];
}
