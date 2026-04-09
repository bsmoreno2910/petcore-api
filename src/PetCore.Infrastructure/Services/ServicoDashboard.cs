using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoDashboard
{
    private readonly AppDbContext _db;

    public ServicoDashboard(AppDbContext db) { _db = db; }

    public async Task<(int Pacientes, int Tutores, int AgendamentosHoje, int InternacoesAtivas,
        int ExamesPendentes, int EstoqueBaixo, decimal ReceitaMes, decimal DespesaMes)>
        ObterResumoAsync(Guid clinicaId)
    {
        var hoje = DateTime.UtcNow.Date;
        var inicioMes = new DateTime(hoje.Year, hoje.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimMes = inicioMes.AddMonths(1);

        var pacientes = await _db.Pacientes.CountAsync(p => p.ClinicaId == clinicaId && p.Ativo);
        var tutores = await _db.Tutores.CountAsync(t => t.ClinicaId == clinicaId && t.Ativo);
        var agendamentosHoje = await _db.Agendamentos.CountAsync(a => a.ClinicaId == clinicaId && a.DataHoraAgendada >= hoje && a.DataHoraAgendada < hoje.AddDays(1));
        var internacoesAtivas = await _db.Internacoes.CountAsync(h => h.ClinicaId == clinicaId && h.Status == StatusInternacao.Ativo);
        var examesPendentes = await _db.SolicitacoesExame.CountAsync(e => e.ClinicaId == clinicaId && (e.Status == StatusExame.Solicitado || e.Status == StatusExame.AmostraColetada));
        var estoqueBaixo = await _db.Produtos.CountAsync(p => p.ClinicaId == clinicaId && p.Ativo && p.EstoqueAtual <= p.EstoqueMinimo);

        var transacoesMes = await _db.TransacoesFinanceiras
            .Where(t => t.ClinicaId == clinicaId && t.Status == StatusTransacao.Pago && t.DataPagamento >= inicioMes && t.DataPagamento < fimMes).ToListAsync();

        return (pacientes, tutores, agendamentosHoje, internacoesAtivas, examesPendentes, estoqueBaixo,
            transacoesMes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.ValorPago ?? t.Valor),
            transacoesMes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.ValorPago ?? t.Valor));
    }
}
