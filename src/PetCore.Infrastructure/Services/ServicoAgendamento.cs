using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoAgendamento
{
    private readonly AppDbContext _db;

    public ServicoAgendamento(AppDbContext db) { _db = db; }

    private IQueryable<Agendamento> QueryBase(Guid clinicaId) =>
        _db.Agendamentos.Include(a => a.Paciente).ThenInclude(p => p.Tutor)
            .Include(a => a.Paciente).ThenInclude(p => p.Especie)
            .Include(a => a.Veterinario).Where(a => a.ClinicaId == clinicaId);

    public async Task<(List<Agendamento> Itens, int Total)> ListarPaginadoAsync(
        Guid clinicaId, int pagina, int tamanhoPagina,
        DateTime? data, Guid? veterinarioId, StatusAgendamento? status, TipoAgendamento? tipo)
    {
        var query = QueryBase(clinicaId);
        if (data.HasValue) { var inicio = data.Value.Date; query = query.Where(a => a.DataHoraAgendada >= inicio && a.DataHoraAgendada < inicio.AddDays(1)); }
        if (veterinarioId.HasValue) query = query.Where(a => a.VeterinarioId == veterinarioId.Value);
        if (status.HasValue) query = query.Where(a => a.Status == status.Value);
        if (tipo.HasValue) query = query.Where(a => a.Tipo == tipo.Value);

        var total = await query.CountAsync();
        var itens = await query.OrderBy(a => a.DataHoraAgendada).Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return (itens, total);
    }

    public async Task<List<Agendamento>> ListarCalendarioAsync(Guid clinicaId, DateTime inicio, DateTime fim) =>
        await QueryBase(clinicaId).Where(a => a.DataHoraAgendada >= inicio && a.DataHoraAgendada <= fim)
            .OrderBy(a => a.DataHoraAgendada).ToListAsync();

    public async Task<Agendamento?> ObterPorIdAsync(Guid id, Guid clinicaId) =>
        await QueryBase(clinicaId).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Agendamento> CriarAsync(Guid clinicaId, Guid pacienteId, Guid? veterinarioId,
        TipoAgendamento tipo, DateTime dataHora, int duracao, string? motivo, string? obs)
    {
        if (veterinarioId.HasValue)
        {
            var fim = dataHora.AddMinutes(duracao);
            var conflito = await _db.Agendamentos.AnyAsync(a =>
                a.ClinicaId == clinicaId && a.VeterinarioId == veterinarioId.Value &&
                a.Status != StatusAgendamento.Cancelado && a.Status != StatusAgendamento.Faltou &&
                a.DataHoraAgendada < fim && a.DataHoraAgendada.AddMinutes(a.DuracaoMinutos) > dataHora);
            if (conflito) throw new InvalidOperationException("Já existe um agendamento neste horário para este veterinário.");
        }

        var agendamento = new Agendamento
        {
            Id = Guid.NewGuid(), ClinicaId = clinicaId, PacienteId = pacienteId,
            VeterinarioId = veterinarioId, Tipo = tipo, Status = StatusAgendamento.Agendado,
            DataHoraAgendada = dataHora, DuracaoMinutos = duracao,
            Motivo = motivo, Observacoes = obs,
            CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };
        _db.Agendamentos.Add(agendamento);
        await _db.SaveChangesAsync();
        return (await ObterPorIdAsync(agendamento.Id, clinicaId))!;
    }

    public async Task<Agendamento?> AlterarStatusAsync(Guid id, Guid clinicaId, StatusAgendamento novoStatus, string? motivoCancelamento = null)
    {
        var ag = await _db.Agendamentos.FirstOrDefaultAsync(a => a.Id == id && a.ClinicaId == clinicaId);
        if (ag == null) return null;

        var validas = ObterTransicoesValidas(ag.Status);
        if (!validas.Contains(novoStatus))
            throw new InvalidOperationException($"Não é possível alterar de '{ag.Status}' para '{novoStatus}'.");

        ag.Status = novoStatus;
        ag.AtualizadoEm = DateTime.UtcNow;
        if (novoStatus == StatusAgendamento.EmAndamento) ag.IniciadoEm = DateTime.UtcNow;
        if (novoStatus == StatusAgendamento.Concluido) ag.FinalizadoEm = DateTime.UtcNow;
        if (novoStatus == StatusAgendamento.Cancelado) ag.MotivoCancelamento = motivoCancelamento;

        await _db.SaveChangesAsync();
        return await ObterPorIdAsync(id, clinicaId);
    }

    public async Task<List<(DateTime Inicio, DateTime Fim)>> ListarHorariosDisponiveisAsync(
        Guid clinicaId, Guid veterinarioId, DateTime data, int duracao = 30)
    {
        var inicio = data.Date.AddHours(8);
        var fimDia = data.Date.AddHours(18);
        var existentes = await _db.Agendamentos
            .Where(a => a.ClinicaId == clinicaId && a.VeterinarioId == veterinarioId &&
                a.DataHoraAgendada >= inicio && a.DataHoraAgendada < fimDia &&
                a.Status != StatusAgendamento.Cancelado && a.Status != StatusAgendamento.Faltou)
            .Select(a => new { a.DataHoraAgendada, a.DuracaoMinutos }).ToListAsync();

        var slots = new List<(DateTime, DateTime)>();
        var atual = inicio;
        while (atual.AddMinutes(duracao) <= fimDia)
        {
            var fimSlot = atual.AddMinutes(duracao);
            if (!existentes.Any(a => a.DataHoraAgendada < fimSlot && a.DataHoraAgendada.AddMinutes(a.DuracaoMinutos) > atual))
                slots.Add((atual, fimSlot));
            atual = atual.AddMinutes(duracao);
        }
        return slots;
    }

    private static HashSet<StatusAgendamento> ObterTransicoesValidas(StatusAgendamento atual) => atual switch
    {
        StatusAgendamento.Agendado => [StatusAgendamento.Confirmado, StatusAgendamento.Cancelado, StatusAgendamento.Faltou],
        StatusAgendamento.Confirmado => [StatusAgendamento.Chegou, StatusAgendamento.Cancelado, StatusAgendamento.Faltou],
        StatusAgendamento.Chegou => [StatusAgendamento.EmAndamento, StatusAgendamento.Cancelado],
        StatusAgendamento.EmAndamento => [StatusAgendamento.Concluido],
        _ => []
    };
}
