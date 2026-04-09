namespace PetCore.Domain.Entities;

public class Clinica
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string? RazaoSocial { get; set; }
    public string? Cnpj { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }

    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }

    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public ICollection<ClinicaUsuario> ClinicaUsuarios { get; set; } = [];
    public ICollection<Paciente> Pacientes { get; set; } = [];
    public ICollection<Produto> Produtos { get; set; } = [];
    public ICollection<Agendamento> Agendamentos { get; set; } = [];
    public ICollection<TransacaoFinanceira> Transacoes { get; set; } = [];
    public ICollection<CentroCusto> CentrosCusto { get; set; } = [];
}
