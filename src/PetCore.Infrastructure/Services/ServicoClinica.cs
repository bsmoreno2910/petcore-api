using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoClinica
{
    private readonly AppDbContext _db;

    public ServicoClinica(AppDbContext db) { _db = db; }

    public async Task<List<Clinica>> ListarTodasAsync() =>
        await _db.Clinicas.OrderBy(c => c.Nome).ToListAsync();

    public async Task<Clinica?> ObterPorIdAsync(Guid id) =>
        await _db.Clinicas.FindAsync(id);

    public async Task<Clinica> CriarAsync(Clinica clinica)
    {
        clinica.Id = Guid.NewGuid();
        clinica.CriadoEm = DateTime.UtcNow;
        clinica.AtualizadoEm = DateTime.UtcNow;
        _db.Clinicas.Add(clinica);
        await _db.SaveChangesAsync();
        return clinica;
    }

    public async Task<Clinica?> AtualizarAsync(Guid id, Action<Clinica> acao)
    {
        var clinica = await _db.Clinicas.FindAsync(id);
        if (clinica == null) return null;
        acao(clinica);
        clinica.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return clinica;
    }

    public async Task<bool> AlternarAtivoAsync(Guid id)
    {
        var clinica = await _db.Clinicas.FindAsync(id);
        if (clinica == null) return false;
        clinica.Ativo = !clinica.Ativo;
        clinica.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ClinicaUsuario>> ListarUsuariosAsync(Guid clinicaId) =>
        await _db.ClinicaUsuarios.Include(cu => cu.Usuario)
            .Where(cu => cu.ClinicaId == clinicaId).ToListAsync();

    public async Task<ClinicaUsuario> AdicionarUsuarioAsync(Guid clinicaId, Guid usuarioId, PerfilUsuario perfil)
    {
        var existe = await _db.ClinicaUsuarios.AnyAsync(cu => cu.ClinicaId == clinicaId && cu.UsuarioId == usuarioId);
        if (existe) throw new InvalidOperationException("Usuário já vinculado a esta clínica.");

        var cu = new ClinicaUsuario
        {
            Id = Guid.NewGuid(), ClinicaId = clinicaId, UsuarioId = usuarioId,
            Perfil = perfil, CriadoEm = DateTime.UtcNow
        };
        _db.ClinicaUsuarios.Add(cu);
        await _db.SaveChangesAsync();
        return await _db.ClinicaUsuarios.Include(x => x.Usuario).FirstAsync(x => x.Id == cu.Id);
    }

    public async Task<bool> RemoverUsuarioAsync(Guid clinicaId, Guid usuarioId)
    {
        var cu = await _db.ClinicaUsuarios.FirstOrDefaultAsync(x => x.ClinicaId == clinicaId && x.UsuarioId == usuarioId);
        if (cu == null) return false;
        _db.ClinicaUsuarios.Remove(cu);
        await _db.SaveChangesAsync();
        return true;
    }
}
