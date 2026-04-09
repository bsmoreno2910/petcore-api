using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ServicoUsuario
{
    private readonly AppDbContext _db;

    public ServicoUsuario(AppDbContext db) { _db = db; }

    public async Task<List<Usuario>> ListarTodosAsync() =>
        await _db.Usuarios.Include(u => u.ClinicaUsuarios).ThenInclude(cu => cu.Clinica)
            .OrderBy(u => u.Nome).ToListAsync();

    public async Task<Usuario?> ObterPorIdAsync(Guid id) =>
        await _db.Usuarios.Include(u => u.ClinicaUsuarios).ThenInclude(cu => cu.Clinica)
            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<Usuario> CriarAsync(string nome, string email, string senha, string? telefone, string? crmv)
    {
        if (await _db.Usuarios.AnyAsync(u => u.Email == email))
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(), Nome = nome, Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
            Telefone = telefone, Crmv = crmv,
            CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario?> AtualizarAsync(Guid id, Action<Usuario> acao)
    {
        var usuario = await _db.Usuarios.Include(u => u.ClinicaUsuarios).ThenInclude(cu => cu.Clinica)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (usuario == null) return null;
        acao(usuario);
        usuario.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> AlternarAtivoAsync(Guid id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario == null) return false;
        usuario.Ativo = !usuario.Ativo;
        usuario.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}
