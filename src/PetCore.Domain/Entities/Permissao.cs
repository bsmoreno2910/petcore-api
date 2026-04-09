namespace PetCore.Domain.Entities;

public class Permissao
{
    public Guid Id { get; set; }
    public string Perfil { get; set; } = string.Empty;
    public string Modulo { get; set; } = string.Empty;
    public bool PodeVisualizar { get; set; }
    public bool PodeAdicionar { get; set; }
    public bool PodeEditar { get; set; }
    public bool PodeExcluir { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}
