using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data.Conventions;

namespace PetCore.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly ITenantProvider? _tenant;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider? tenant = null) : base(options)
    {
        _tenant = tenant;
    }

    // Módulo 1: Autenticação & Multiclínica
    public DbSet<Clinica> Clinicas => Set<Clinica>();
    public DbSet<ClinicaUsuario> ClinicaUsuarios => Set<ClinicaUsuario>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    // Módulo 2: Cadastros Base
    public DbSet<Especie> Especies => Set<Especie>();
    public DbSet<Raca> Racas => Set<Raca>();
    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();

    // Módulo 3: Agenda
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();

    // Módulo 4: Prontuário & Atendimento
    public DbSet<Prontuario> Prontuarios => Set<Prontuario>();
    public DbSet<Prescricao> Prescricoes => Set<Prescricao>();
    public DbSet<Internacao> Internacoes => Set<Internacao>();
    public DbSet<Evolucao> Evolucoes => Set<Evolucao>();

    // Módulo 5: Exames
    public DbSet<TipoExame> TiposExame => Set<TipoExame>();
    public DbSet<SolicitacaoExame> SolicitacoesExame => Set<SolicitacaoExame>();
    public DbSet<ResultadoExame> ResultadosExame => Set<ResultadoExame>();

    // Módulo 6: Almoxarifado
    public DbSet<CategoriaProduto> CategoriasProduto => Set<CategoriaProduto>();
    public DbSet<UnidadeProduto> UnidadesProduto => Set<UnidadeProduto>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Movimentacao> Movimentacoes => Set<Movimentacao>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    // Módulo 7: Financeiro
    public DbSet<CategoriaFinanceira> CategoriasFinanceiras => Set<CategoriaFinanceira>();
    public DbSet<TransacaoFinanceira> TransacoesFinanceiras => Set<TransacaoFinanceira>();
    public DbSet<ParcelaTransacao> ParcelasTransacao => Set<ParcelaTransacao>();

    // Módulo 8: Custos
    public DbSet<CentroCusto> CentrosCusto => Set<CentroCusto>();

    // Refresh Tokens
    public DbSet<TokenAtualizacao> TokensAtualizacao => Set<TokenAtualizacao>();

    // Módulo 10: Auditoria
    public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();

    // Módulo 11: Permissões
    public DbSet<Permissao> Permissoes => Set<Permissao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global Query Filters para Tenant Isolation
        // SuperAdmin ignora os filtros; durante migrations _tenant é null
        var clinicaId = _tenant?.ClinicaId ?? Guid.Empty;
        var ehSuperAdmin = _tenant?.EhSuperAdmin ?? true;

        if (!ehSuperAdmin && clinicaId != Guid.Empty)
        {
            modelBuilder.Entity<Tutor>().HasQueryFilter(t => t.ClinicaId == clinicaId);
            modelBuilder.Entity<Paciente>().HasQueryFilter(p => p.ClinicaId == clinicaId);
            modelBuilder.Entity<Agendamento>().HasQueryFilter(a => a.ClinicaId == clinicaId);
            modelBuilder.Entity<Prontuario>().HasQueryFilter(m => m.ClinicaId == clinicaId);
            modelBuilder.Entity<Internacao>().HasQueryFilter(h => h.ClinicaId == clinicaId);
            modelBuilder.Entity<SolicitacaoExame>().HasQueryFilter(e => e.ClinicaId == clinicaId);
            modelBuilder.Entity<Produto>().HasQueryFilter(p => p.ClinicaId == clinicaId);
            modelBuilder.Entity<Movimentacao>().HasQueryFilter(m => m.ClinicaId == clinicaId);
            modelBuilder.Entity<Pedido>().HasQueryFilter(o => o.ClinicaId == clinicaId);
            modelBuilder.Entity<TransacaoFinanceira>().HasQueryFilter(t => t.ClinicaId == clinicaId);
            modelBuilder.Entity<CentroCusto>().HasQueryFilter(c => c.ClinicaId == clinicaId);
            modelBuilder.Entity<LogAuditoria>().HasQueryFilter(l => l.ClinicaId == clinicaId);
        }

        // Aplicar camelCase em todas as tabelas e colunas
        modelBuilder.ApplyNomenclaturasCamelCase();

        base.OnModelCreating(modelBuilder);
    }
}
