using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;

namespace PetCore.Infrastructure.Data.Seed;

public static class SemeadorDados
{
    public static async Task SemearAsync(AppDbContext db)
    {
        if (await db.Usuarios.AnyAsync())
            return;

        var superAdmin = new Usuario
        {
            Id = Guid.NewGuid(), Nome = "Administrador PetCore", Email = "admin@petcore.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Ativo = true, CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };
        var vet = new Usuario
        {
            Id = Guid.NewGuid(), Nome = "Dr. João Silva", Email = "joao@petcore.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Vet@123"), Crmv = "CRMV-SP 12345",
            Ativo = true, CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };
        var recepcionista = new Usuario
        {
            Id = Guid.NewGuid(), Nome = "Maria Santos", Email = "maria@petcore.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Rec@123"),
            Ativo = true, CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };
        db.Usuarios.AddRange(superAdmin, vet, recepcionista);

        var clinica = new Clinica
        {
            Id = Guid.NewGuid(), Nome = "PetCore Clínica Central", NomeFantasia = "PetCore Central",
            Telefone = "(11) 99999-0000", Email = "contato@petcore.com",
            Cidade = "São Paulo", Estado = "SP",
            Ativo = true, CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow
        };
        db.Clinicas.Add(clinica);

        db.ClinicaUsuarios.AddRange(
            new ClinicaUsuario { Id = Guid.NewGuid(), ClinicaId = clinica.Id, UsuarioId = superAdmin.Id, Perfil = PerfilUsuario.SuperAdmin, CriadoEm = DateTime.UtcNow },
            new ClinicaUsuario { Id = Guid.NewGuid(), ClinicaId = clinica.Id, UsuarioId = vet.Id, Perfil = PerfilUsuario.Veterinario, CriadoEm = DateTime.UtcNow },
            new ClinicaUsuario { Id = Guid.NewGuid(), ClinicaId = clinica.Id, UsuarioId = recepcionista.Id, Perfil = PerfilUsuario.Recepcionista, CriadoEm = DateTime.UtcNow }
        );

        var canino = new Especie { Id = Guid.NewGuid(), Nome = "Canino" };
        var felino = new Especie { Id = Guid.NewGuid(), Nome = "Felino" };
        var ave = new Especie { Id = Guid.NewGuid(), Nome = "Ave" };
        var reptil = new Especie { Id = Guid.NewGuid(), Nome = "Réptil" };
        var roedor = new Especie { Id = Guid.NewGuid(), Nome = "Roedor" };
        db.Especies.AddRange(canino, felino, ave, reptil, roedor);

        db.Racas.AddRange(
            new Raca { Id = Guid.NewGuid(), EspecieId = canino.Id, Nome = "Labrador Retriever" },
            new Raca { Id = Guid.NewGuid(), EspecieId = canino.Id, Nome = "Golden Retriever" },
            new Raca { Id = Guid.NewGuid(), EspecieId = canino.Id, Nome = "Bulldog" },
            new Raca { Id = Guid.NewGuid(), EspecieId = canino.Id, Nome = "Poodle" },
            new Raca { Id = Guid.NewGuid(), EspecieId = canino.Id, Nome = "Pastor Alemão" },
            new Raca { Id = Guid.NewGuid(), EspecieId = canino.Id, Nome = "SRD (Sem Raça Definida)" },
            new Raca { Id = Guid.NewGuid(), EspecieId = felino.Id, Nome = "Siamês" },
            new Raca { Id = Guid.NewGuid(), EspecieId = felino.Id, Nome = "Persa" },
            new Raca { Id = Guid.NewGuid(), EspecieId = felino.Id, Nome = "Maine Coon" },
            new Raca { Id = Guid.NewGuid(), EspecieId = felino.Id, Nome = "SRD (Sem Raça Definida)" },
            new Raca { Id = Guid.NewGuid(), EspecieId = ave.Id, Nome = "Calopsita" },
            new Raca { Id = Guid.NewGuid(), EspecieId = ave.Id, Nome = "Periquito" },
            new Raca { Id = Guid.NewGuid(), EspecieId = roedor.Id, Nome = "Hamster" },
            new Raca { Id = Guid.NewGuid(), EspecieId = roedor.Id, Nome = "Porquinho-da-índia" }
        );

        db.CategoriasProduto.AddRange(
            new CategoriaProduto { Id = Guid.NewGuid(), Nome = "Medicamentos", Cor = "#3b82f6", CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow },
            new CategoriaProduto { Id = Guid.NewGuid(), Nome = "Materiais Cirúrgicos", Cor = "#ef4444", CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow },
            new CategoriaProduto { Id = Guid.NewGuid(), Nome = "Materiais de Limpeza", Cor = "#22c55e", CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow },
            new CategoriaProduto { Id = Guid.NewGuid(), Nome = "Vacinas", Cor = "#a855f7", CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow },
            new CategoriaProduto { Id = Guid.NewGuid(), Nome = "Insumos Laboratoriais", Cor = "#f97316", CriadoEm = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow }
        );

        db.UnidadesProduto.AddRange(
            new UnidadeProduto { Id = Guid.NewGuid(), Sigla = "un", Nome = "Unidade", CriadoEm = DateTime.UtcNow },
            new UnidadeProduto { Id = Guid.NewGuid(), Sigla = "cx", Nome = "Caixa", CriadoEm = DateTime.UtcNow },
            new UnidadeProduto { Id = Guid.NewGuid(), Sigla = "fr", Nome = "Frasco", CriadoEm = DateTime.UtcNow },
            new UnidadeProduto { Id = Guid.NewGuid(), Sigla = "amp", Nome = "Ampola", CriadoEm = DateTime.UtcNow },
            new UnidadeProduto { Id = Guid.NewGuid(), Sigla = "ml", Nome = "Mililitro", CriadoEm = DateTime.UtcNow },
            new UnidadeProduto { Id = Guid.NewGuid(), Sigla = "kg", Nome = "Quilograma", CriadoEm = DateTime.UtcNow }
        );

        db.TiposExame.AddRange(
            new TipoExame { Id = Guid.NewGuid(), Nome = "Hemograma Completo", Categoria = "Laboratorial", PrecoDefault = 80m, CriadoEm = DateTime.UtcNow },
            new TipoExame { Id = Guid.NewGuid(), Nome = "Bioquímico", Categoria = "Laboratorial", PrecoDefault = 120m, CriadoEm = DateTime.UtcNow },
            new TipoExame { Id = Guid.NewGuid(), Nome = "Raio-X", Categoria = "Imagem", PrecoDefault = 150m, CriadoEm = DateTime.UtcNow },
            new TipoExame { Id = Guid.NewGuid(), Nome = "Ultrassonografia", Categoria = "Imagem", PrecoDefault = 200m, CriadoEm = DateTime.UtcNow },
            new TipoExame { Id = Guid.NewGuid(), Nome = "Urinálise", Categoria = "Laboratorial", PrecoDefault = 60m, CriadoEm = DateTime.UtcNow }
        );

        await db.SaveChangesAsync();
    }
}
