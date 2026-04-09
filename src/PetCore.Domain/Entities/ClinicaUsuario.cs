using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class ClinicaUsuario
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public Guid UsuarioId { get; set; }
    public PerfilUsuario Perfil { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }

    public Clinica Clinica { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
