namespace PetCore.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Crmv { get; set; }
    public string? AvatarUrl { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public ICollection<ClinicaUsuario> ClinicaUsuarios { get; set; } = [];
    public ICollection<Movimentacao> MovimentacoesCriadas { get; set; } = [];
    public ICollection<Movimentacao> MovimentacoesAprovadas { get; set; } = [];
    public ICollection<Pedido> PedidosCriados { get; set; } = [];
    public ICollection<Pedido> PedidosAprovados { get; set; } = [];
    public ICollection<Prontuario> Prontuarios { get; set; } = [];
    public ICollection<Agendamento> AgendamentosComoVet { get; set; } = [];
    public ICollection<LogAuditoria> LogsAuditoria { get; set; } = [];
}
