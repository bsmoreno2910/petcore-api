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

        // ── Fixed GUIDs ──────────────────────────────────────────────────
        // Users
        var adminId       = Guid.Parse("a0000000-0000-0000-0000-000000000001");
        var vetId         = Guid.Parse("a0000000-0000-0000-0000-000000000002");
        var recepId       = Guid.Parse("a0000000-0000-0000-0000-000000000003");
        var operadorId    = Guid.Parse("a0000000-0000-0000-0000-000000000004");
        var financeiroId  = Guid.Parse("a0000000-0000-0000-0000-000000000005");

        // Clinics
        var clinica1Id = Guid.Parse("b0000000-0000-0000-0000-000000000001");
        var clinica2Id = Guid.Parse("b0000000-0000-0000-0000-000000000002");

        // Species
        var caninoId = Guid.Parse("c0000000-0000-0000-0000-000000000001");
        var felinoId = Guid.Parse("c0000000-0000-0000-0000-000000000002");
        var aveId    = Guid.Parse("c0000000-0000-0000-0000-000000000003");
        var reptilId = Guid.Parse("c0000000-0000-0000-0000-000000000004");
        var roedorId = Guid.Parse("c0000000-0000-0000-0000-000000000005");

        // Breeds
        var labradorId    = Guid.Parse("d0000000-0000-0000-0000-000000000001");
        var goldenId      = Guid.Parse("d0000000-0000-0000-0000-000000000002");
        var bulldogId     = Guid.Parse("d0000000-0000-0000-0000-000000000003");
        var poodleId      = Guid.Parse("d0000000-0000-0000-0000-000000000004");
        var pastorId      = Guid.Parse("d0000000-0000-0000-0000-000000000005");
        var srdCaninoId   = Guid.Parse("d0000000-0000-0000-0000-000000000006");
        var siamesId      = Guid.Parse("d0000000-0000-0000-0000-000000000007");
        var persaId       = Guid.Parse("d0000000-0000-0000-0000-000000000008");
        var maineCoonId   = Guid.Parse("d0000000-0000-0000-0000-000000000009");
        var srdFelinoId   = Guid.Parse("d0000000-0000-0000-0000-00000000000a");
        var calopsitaId   = Guid.Parse("d0000000-0000-0000-0000-00000000000b");
        var periquitoId   = Guid.Parse("d0000000-0000-0000-0000-00000000000c");
        var hamsterId     = Guid.Parse("d0000000-0000-0000-0000-00000000000d");
        var porquinhoId   = Guid.Parse("d0000000-0000-0000-0000-00000000000e");

        // Tutors
        var tutorAnaId      = Guid.Parse("e0000000-0000-0000-0000-000000000001");
        var tutorRobertoId  = Guid.Parse("e0000000-0000-0000-0000-000000000002");
        var tutorMarianaId  = Guid.Parse("e0000000-0000-0000-0000-000000000003");
        var tutorPedroId    = Guid.Parse("e0000000-0000-0000-0000-000000000004");
        var tutorJulianaId  = Guid.Parse("e0000000-0000-0000-0000-000000000005");
        var tutorLucasId    = Guid.Parse("e0000000-0000-0000-0000-000000000006");
        var tutorCamilaId   = Guid.Parse("e0000000-0000-0000-0000-000000000007");
        var tutorThiagoId   = Guid.Parse("e0000000-0000-0000-0000-000000000008");
        var tutorPatriciaId = Guid.Parse("e0000000-0000-0000-0000-000000000009");
        var tutorBrunoId    = Guid.Parse("e0000000-0000-0000-0000-00000000000a");

        // Patients
        var pac01 = Guid.Parse("f0000000-0000-0000-0000-000000000001");
        var pac02 = Guid.Parse("f0000000-0000-0000-0000-000000000002");
        var pac03 = Guid.Parse("f0000000-0000-0000-0000-000000000003");
        var pac04 = Guid.Parse("f0000000-0000-0000-0000-000000000004");
        var pac05 = Guid.Parse("f0000000-0000-0000-0000-000000000005");
        var pac06 = Guid.Parse("f0000000-0000-0000-0000-000000000006");
        var pac07 = Guid.Parse("f0000000-0000-0000-0000-000000000007");
        var pac08 = Guid.Parse("f0000000-0000-0000-0000-000000000008");
        var pac09 = Guid.Parse("f0000000-0000-0000-0000-000000000009");
        var pac10 = Guid.Parse("f0000000-0000-0000-0000-00000000000a");
        var pac11 = Guid.Parse("f0000000-0000-0000-0000-00000000000b");
        var pac12 = Guid.Parse("f0000000-0000-0000-0000-00000000000c");
        var pac13 = Guid.Parse("f0000000-0000-0000-0000-00000000000d");
        var pac14 = Guid.Parse("f0000000-0000-0000-0000-00000000000e");
        var pac15 = Guid.Parse("f0000000-0000-0000-0000-00000000000f");
        var pac16 = Guid.Parse("f0000000-0000-0000-0000-000000000010");
        var pac17 = Guid.Parse("f0000000-0000-0000-0000-000000000011");
        var pac18 = Guid.Parse("f0000000-0000-0000-0000-000000000012");
        var pac19 = Guid.Parse("f0000000-0000-0000-0000-000000000013");
        var pac20 = Guid.Parse("f0000000-0000-0000-0000-000000000014");

        // Product categories (existing)
        var catMedicamentosId   = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var catCirurgicosId     = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var catLimpezaId        = Guid.Parse("10000000-0000-0000-0000-000000000003");
        var catVacinasId        = Guid.Parse("10000000-0000-0000-0000-000000000004");
        var catLaboratoriaisId  = Guid.Parse("10000000-0000-0000-0000-000000000005");

        // Units
        var unidadeUnId  = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var unidadeCxId  = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var unidadeFrId  = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var unidadeAmpId = Guid.Parse("20000000-0000-0000-0000-000000000004");
        var unidadeMlId  = Guid.Parse("20000000-0000-0000-0000-000000000005");
        var unidadeKgId  = Guid.Parse("20000000-0000-0000-0000-000000000006");

        // Financial categories
        var finCatConsultaId   = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var finCatCirurgiaId   = Guid.Parse("30000000-0000-0000-0000-000000000002");
        var finCatVacinacaoId  = Guid.Parse("30000000-0000-0000-0000-000000000003");
        var finCatMedicId      = Guid.Parse("30000000-0000-0000-0000-000000000004");
        var finCatAluguelId    = Guid.Parse("30000000-0000-0000-0000-000000000005");
        var finCatSalariosId   = Guid.Parse("30000000-0000-0000-0000-000000000006");

        // Products
        var prodVacinaV10Id    = Guid.Parse("40000000-0000-0000-0000-000000000001");
        var prodNexgardId      = Guid.Parse("40000000-0000-0000-0000-000000000002");
        var prodRoyalCaninId   = Guid.Parse("40000000-0000-0000-0000-000000000003");
        var prodSeringaId      = Guid.Parse("40000000-0000-0000-0000-000000000004");
        var prodLuvaId         = Guid.Parse("40000000-0000-0000-0000-000000000005");

        // Appointments
        var agend01 = Guid.Parse("50000000-0000-0000-0000-000000000001");
        var agend02 = Guid.Parse("50000000-0000-0000-0000-000000000002");
        var agend03 = Guid.Parse("50000000-0000-0000-0000-000000000003");
        var agend04 = Guid.Parse("50000000-0000-0000-0000-000000000004");
        var agend05 = Guid.Parse("50000000-0000-0000-0000-000000000005");
        var agend06 = Guid.Parse("50000000-0000-0000-0000-000000000006");
        var agend07 = Guid.Parse("50000000-0000-0000-0000-000000000007");
        var agend08 = Guid.Parse("50000000-0000-0000-0000-000000000008");

        var now = DateTime.UtcNow;

        // ══════════════════════════════════════════════════════════════════
        // 1. USERS (5 total)
        // ══════════════════════════════════════════════════════════════════
        var superAdmin = new Usuario
        {
            Id = adminId, Nome = "Administrador PetCore", Email = "admin@petcore.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Ativo = true, CriadoEm = now, AtualizadoEm = now
        };
        var vet = new Usuario
        {
            Id = vetId, Nome = "Dr. João Silva", Email = "joao@petcore.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Vet@123"), Crmv = "CRMV-SP 12345",
            Ativo = true, CriadoEm = now, AtualizadoEm = now
        };
        var recepcionista = new Usuario
        {
            Id = recepId, Nome = "Maria Santos", Email = "maria@petcore.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Rec@123"),
            Ativo = true, CriadoEm = now, AtualizadoEm = now
        };
        var operador = new Usuario
        {
            Id = operadorId, Nome = "Carlos Silva", Email = "carlos@petcore.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Operador@123"),
            Ativo = true, CriadoEm = now, AtualizadoEm = now
        };
        var financeiro = new Usuario
        {
            Id = financeiroId, Nome = "Fernanda Lima", Email = "fernanda@petcore.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Fin@12345"),
            Ativo = true, CriadoEm = now, AtualizadoEm = now
        };
        db.Usuarios.AddRange(superAdmin, vet, recepcionista, operador, financeiro);

        // ══════════════════════════════════════════════════════════════════
        // 2. CLINICS (2 total)
        // ══════════════════════════════════════════════════════════════════
        var clinica1 = new Clinica
        {
            Id = clinica1Id, Nome = "PetCore Clínica Central", NomeFantasia = "PetCore Central",
            Telefone = "(11) 99999-0000", Email = "contato@petcore.com",
            Cidade = "São Paulo", Estado = "SP",
            Ativo = true, CriadoEm = now, AtualizadoEm = now
        };
        var clinica2 = new Clinica
        {
            Id = clinica2Id, Nome = "PetCore Filial Centro", NomeFantasia = "PetCore Centro",
            Cnpj = "98.765.432/0001-10", Telefone = "(11) 3333-4444",
            Email = "centro@petcore.com",
            Cidade = "São Paulo", Estado = "SP", Bairro = "Centro",
            Ativo = true, CriadoEm = now, AtualizadoEm = now
        };
        db.Clinicas.AddRange(clinica1, clinica2);

        // ══════════════════════════════════════════════════════════════════
        // 3. CLINIC-USER links
        // ══════════════════════════════════════════════════════════════════
        db.ClinicaUsuarios.AddRange(
            // Clinic 1
            new ClinicaUsuario { Id = Guid.Parse("ab000000-0000-0000-0000-000000000001"), ClinicaId = clinica1Id, UsuarioId = adminId,      Perfil = PerfilUsuario.SuperAdmin,    CriadoEm = now },
            new ClinicaUsuario { Id = Guid.Parse("ab000000-0000-0000-0000-000000000002"), ClinicaId = clinica1Id, UsuarioId = vetId,        Perfil = PerfilUsuario.Veterinario,   CriadoEm = now },
            new ClinicaUsuario { Id = Guid.Parse("ab000000-0000-0000-0000-000000000003"), ClinicaId = clinica1Id, UsuarioId = recepId,      Perfil = PerfilUsuario.Recepcionista,  CriadoEm = now },
            new ClinicaUsuario { Id = Guid.Parse("ab000000-0000-0000-0000-000000000004"), ClinicaId = clinica1Id, UsuarioId = operadorId,   Perfil = PerfilUsuario.Operador,       CriadoEm = now },
            new ClinicaUsuario { Id = Guid.Parse("ab000000-0000-0000-0000-000000000005"), ClinicaId = clinica1Id, UsuarioId = financeiroId, Perfil = PerfilUsuario.Financeiro,     CriadoEm = now },
            // Clinic 2 (admin + joao)
            new ClinicaUsuario { Id = Guid.Parse("ab000000-0000-0000-0000-000000000006"), ClinicaId = clinica2Id, UsuarioId = adminId,      Perfil = PerfilUsuario.SuperAdmin,    CriadoEm = now },
            new ClinicaUsuario { Id = Guid.Parse("ab000000-0000-0000-0000-000000000007"), ClinicaId = clinica2Id, UsuarioId = vetId,        Perfil = PerfilUsuario.Veterinario,   CriadoEm = now }
        );

        // ══════════════════════════════════════════════════════════════════
        // 4. SPECIES
        // ══════════════════════════════════════════════════════════════════
        var canino = new Especie { Id = caninoId, Nome = "Canino" };
        var felino = new Especie { Id = felinoId, Nome = "Felino" };
        var ave    = new Especie { Id = aveId,    Nome = "Ave" };
        var reptil = new Especie { Id = reptilId, Nome = "Réptil" };
        var roedor = new Especie { Id = roedorId, Nome = "Roedor" };
        db.Especies.AddRange(canino, felino, ave, reptil, roedor);

        // ══════════════════════════════════════════════════════════════════
        // 5. BREEDS
        // ══════════════════════════════════════════════════════════════════
        db.Racas.AddRange(
            new Raca { Id = labradorId,  EspecieId = caninoId, Nome = "Labrador Retriever" },
            new Raca { Id = goldenId,    EspecieId = caninoId, Nome = "Golden Retriever" },
            new Raca { Id = bulldogId,   EspecieId = caninoId, Nome = "Bulldog" },
            new Raca { Id = poodleId,    EspecieId = caninoId, Nome = "Poodle" },
            new Raca { Id = pastorId,    EspecieId = caninoId, Nome = "Pastor Alemão" },
            new Raca { Id = srdCaninoId, EspecieId = caninoId, Nome = "SRD (Sem Raça Definida)" },
            new Raca { Id = siamesId,    EspecieId = felinoId, Nome = "Siamês" },
            new Raca { Id = persaId,     EspecieId = felinoId, Nome = "Persa" },
            new Raca { Id = maineCoonId, EspecieId = felinoId, Nome = "Maine Coon" },
            new Raca { Id = srdFelinoId, EspecieId = felinoId, Nome = "SRD (Sem Raça Definida)" },
            new Raca { Id = calopsitaId, EspecieId = aveId,    Nome = "Calopsita" },
            new Raca { Id = periquitoId, EspecieId = aveId,    Nome = "Periquito" },
            new Raca { Id = hamsterId,   EspecieId = roedorId, Nome = "Hamster" },
            new Raca { Id = porquinhoId, EspecieId = roedorId, Nome = "Porquinho-da-índia" }
        );

        // ══════════════════════════════════════════════════════════════════
        // 6. PRODUCT CATEGORIES
        // ══════════════════════════════════════════════════════════════════
        db.CategoriasProduto.AddRange(
            new CategoriaProduto { Id = catMedicamentosId,  Nome = "Medicamentos",          Cor = "#3b82f6", CriadoEm = now, AtualizadoEm = now },
            new CategoriaProduto { Id = catCirurgicosId,    Nome = "Materiais Cirúrgicos",  Cor = "#ef4444", CriadoEm = now, AtualizadoEm = now },
            new CategoriaProduto { Id = catLimpezaId,       Nome = "Materiais de Limpeza",  Cor = "#22c55e", CriadoEm = now, AtualizadoEm = now },
            new CategoriaProduto { Id = catVacinasId,       Nome = "Vacinas",               Cor = "#a855f7", CriadoEm = now, AtualizadoEm = now },
            new CategoriaProduto { Id = catLaboratoriaisId, Nome = "Insumos Laboratoriais", Cor = "#f97316", CriadoEm = now, AtualizadoEm = now }
        );

        // ══════════════════════════════════════════════════════════════════
        // 7. UNITS
        // ══════════════════════════════════════════════════════════════════
        db.UnidadesProduto.AddRange(
            new UnidadeProduto { Id = unidadeUnId,  Sigla = "un",  Nome = "Unidade",     CriadoEm = now },
            new UnidadeProduto { Id = unidadeCxId,  Sigla = "cx",  Nome = "Caixa",       CriadoEm = now },
            new UnidadeProduto { Id = unidadeFrId,  Sigla = "fr",  Nome = "Frasco",      CriadoEm = now },
            new UnidadeProduto { Id = unidadeAmpId, Sigla = "amp", Nome = "Ampola",      CriadoEm = now },
            new UnidadeProduto { Id = unidadeMlId,  Sigla = "ml",  Nome = "Mililitro",   CriadoEm = now },
            new UnidadeProduto { Id = unidadeKgId,  Sigla = "kg",  Nome = "Quilograma",  CriadoEm = now }
        );

        // ══════════════════════════════════════════════════════════════════
        // 8. EXAM TYPES
        // ══════════════════════════════════════════════════════════════════
        db.TiposExame.AddRange(
            new TipoExame { Id = Guid.Parse("60000000-0000-0000-0000-000000000001"), Nome = "Hemograma Completo", Categoria = "Laboratorial", PrecoDefault = 80m,  CriadoEm = now },
            new TipoExame { Id = Guid.Parse("60000000-0000-0000-0000-000000000002"), Nome = "Bioquímico",         Categoria = "Laboratorial", PrecoDefault = 120m, CriadoEm = now },
            new TipoExame { Id = Guid.Parse("60000000-0000-0000-0000-000000000003"), Nome = "Raio-X",             Categoria = "Imagem",       PrecoDefault = 150m, CriadoEm = now },
            new TipoExame { Id = Guid.Parse("60000000-0000-0000-0000-000000000004"), Nome = "Ultrassonografia",   Categoria = "Imagem",       PrecoDefault = 200m, CriadoEm = now },
            new TipoExame { Id = Guid.Parse("60000000-0000-0000-0000-000000000005"), Nome = "Urinálise",          Categoria = "Laboratorial", PrecoDefault = 60m,  CriadoEm = now }
        );

        // ══════════════════════════════════════════════════════════════════
        // 9. TUTORS (10)
        // ══════════════════════════════════════════════════════════════════
        db.Tutores.AddRange(
            new Tutor
            {
                Id = tutorAnaId, ClinicaId = clinica1Id, Nome = "Ana Santos",
                Cpf = "123.456.789-09", Telefone = "(11) 98111-1001", Email = "ana.santos@email.com",
                Rua = "Rua Augusta", Numero = "1200", Bairro = "Consolação", Cidade = "São Paulo", Estado = "SP", Cep = "01304-001",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Tutor
            {
                Id = tutorRobertoId, ClinicaId = clinica1Id, Nome = "Roberto Oliveira",
                Cpf = "234.567.890-12", Telefone = "(11) 98111-1002", Email = "roberto.oliveira@email.com",
                Rua = "Av. Paulista", Numero = "900", Bairro = "Bela Vista", Cidade = "São Paulo", Estado = "SP", Cep = "01310-100",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Tutor
            {
                Id = tutorMarianaId, ClinicaId = clinica1Id, Nome = "Mariana Costa",
                Cpf = "345.678.901-23", Telefone = "(11) 98111-1003", Email = "mariana.costa@email.com",
                Rua = "Rua Oscar Freire", Numero = "450", Bairro = "Jardins", Cidade = "São Paulo", Estado = "SP", Cep = "01426-001",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Tutor
            {
                Id = tutorPedroId, ClinicaId = clinica1Id, Nome = "Pedro Almeida",
                Cpf = "456.789.012-34", Telefone = "(11) 98111-1004", Email = "pedro.almeida@email.com",
                Rua = "Rua da Consolação", Numero = "2100", Bairro = "Consolação", Cidade = "São Paulo", Estado = "SP", Cep = "01301-100",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Tutor
            {
                Id = tutorJulianaId, ClinicaId = clinica1Id, Nome = "Juliana Ferreira",
                Cpf = "567.890.123-45", Telefone = "(11) 98111-1005", Email = "juliana.ferreira@email.com",
                Rua = "Alameda Santos", Numero = "800", Bairro = "Cerqueira César", Cidade = "São Paulo", Estado = "SP", Cep = "01418-100",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Tutor
            {
                Id = tutorLucasId, ClinicaId = clinica1Id, Nome = "Lucas Mendes",
                Cpf = "678.901.234-56", Telefone = "(11) 98111-1006", Email = "lucas.mendes@email.com",
                Rua = "Rua Haddock Lobo", Numero = "595", Bairro = "Cerqueira César", Cidade = "São Paulo", Estado = "SP", Cep = "01414-001",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Tutor
            {
                Id = tutorCamilaId, ClinicaId = clinica1Id, Nome = "Camila Souza",
                Cpf = "789.012.345-67", Telefone = "(11) 98111-1007", Email = "camila.souza@email.com",
                Rua = "Av. Rebouças", Numero = "1300", Bairro = "Pinheiros", Cidade = "São Paulo", Estado = "SP", Cep = "05401-100",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Tutor
            {
                Id = tutorThiagoId, ClinicaId = clinica1Id, Nome = "Thiago Ribeiro",
                Cpf = "890.123.456-78", Telefone = "(11) 98111-1008", Email = "thiago.ribeiro@email.com",
                Rua = "Rua dos Pinheiros", Numero = "700", Bairro = "Pinheiros", Cidade = "São Paulo", Estado = "SP", Cep = "05422-001",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Tutor
            {
                Id = tutorPatriciaId, ClinicaId = clinica1Id, Nome = "Patricia Lima",
                Cpf = "901.234.567-89", Telefone = "(11) 98111-1009", Email = "patricia.lima@email.com",
                Rua = "Rua Frei Caneca", Numero = "250", Bairro = "Consolação", Cidade = "São Paulo", Estado = "SP", Cep = "01307-001",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Tutor
            {
                Id = tutorBrunoId, ClinicaId = clinica1Id, Nome = "Bruno Carvalho",
                Cpf = "012.345.678-90", Telefone = "(11) 98111-1010", Email = "bruno.carvalho@email.com",
                Rua = "Av. Brigadeiro Faria Lima", Numero = "1800", Bairro = "Jardim Paulistano", Cidade = "São Paulo", Estado = "SP", Cep = "01451-001",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            }
        );

        // ══════════════════════════════════════════════════════════════════
        // 10. PATIENTS (20)
        // ══════════════════════════════════════════════════════════════════
        db.Pacientes.AddRange(
            // Ana Santos - 2 pets
            new Paciente
            {
                Id = pac01, ClinicaId = clinica1Id, TutorId = tutorAnaId, EspecieId = caninoId, RacaId = labradorId,
                Nome = "Thor", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Peso = 32.5m, Cor = "Dourado", Castrado = true,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Paciente
            {
                Id = pac02, ClinicaId = clinica1Id, TutorId = tutorAnaId, EspecieId = felinoId, RacaId = siamesId,
                Nome = "Mia", Sexo = SexoPaciente.Femea, DataNascimento = new DateTime(2022, 7, 20, 0, 0, 0, DateTimeKind.Utc),
                Peso = 4.2m, Cor = "Creme com pontas escuras", Castrado = true, Alergias = "Frango",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            // Roberto Oliveira - 2 pets
            new Paciente
            {
                Id = pac03, ClinicaId = clinica1Id, TutorId = tutorRobertoId, EspecieId = caninoId, RacaId = goldenId,
                Nome = "Buddy", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2020, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                Peso = 35.0m, Cor = "Dourado", Castrado = false,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Paciente
            {
                Id = pac04, ClinicaId = clinica1Id, TutorId = tutorRobertoId, EspecieId = caninoId, RacaId = bulldogId,
                Nome = "Luna", Sexo = SexoPaciente.Femea, DataNascimento = new DateTime(2023, 5, 8, 0, 0, 0, DateTimeKind.Utc),
                Peso = 22.0m, Cor = "Branco e caramelo", Castrado = false, Alergias = "Glúten",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            // Mariana Costa - 2 pets
            new Paciente
            {
                Id = pac05, ClinicaId = clinica1Id, TutorId = tutorMarianaId, EspecieId = felinoId, RacaId = persaId,
                Nome = "Princesa", Sexo = SexoPaciente.Femea, DataNascimento = new DateTime(2019, 11, 25, 0, 0, 0, DateTimeKind.Utc),
                Peso = 5.1m, Cor = "Branco", Castrado = true,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Paciente
            {
                Id = pac06, ClinicaId = clinica1Id, TutorId = tutorMarianaId, EspecieId = felinoId, RacaId = maineCoonId,
                Nome = "Simba", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2021, 8, 3, 0, 0, 0, DateTimeKind.Utc),
                Peso = 8.5m, Cor = "Tigrado", Castrado = true,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            // Pedro Almeida - 2 pets
            new Paciente
            {
                Id = pac07, ClinicaId = clinica1Id, TutorId = tutorPedroId, EspecieId = caninoId, RacaId = pastorId,
                Nome = "Rex", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2020, 6, 12, 0, 0, 0, DateTimeKind.Utc),
                Peso = 38.0m, Cor = "Preto e marrom", Castrado = true,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Paciente
            {
                Id = pac08, ClinicaId = clinica1Id, TutorId = tutorPedroId, EspecieId = aveId, RacaId = calopsitaId,
                Nome = "Piu", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2023, 2, 14, 0, 0, 0, DateTimeKind.Utc),
                Peso = 0.09m, Cor = "Cinza com bochechas laranjas", Castrado = false,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            // Juliana Ferreira - 2 pets
            new Paciente
            {
                Id = pac09, ClinicaId = clinica1Id, TutorId = tutorJulianaId, EspecieId = caninoId, RacaId = poodleId,
                Nome = "Mel", Sexo = SexoPaciente.Femea, DataNascimento = new DateTime(2022, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                Peso = 6.0m, Cor = "Branco", Castrado = true, Alergias = "Dipirona",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Paciente
            {
                Id = pac10, ClinicaId = clinica1Id, TutorId = tutorJulianaId, EspecieId = roedorId, RacaId = hamsterId,
                Nome = "Bolinha", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                Peso = 0.04m, Cor = "Caramelo", Castrado = false,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            // Lucas Mendes - 2 pets
            new Paciente
            {
                Id = pac11, ClinicaId = clinica1Id, TutorId = tutorLucasId, EspecieId = caninoId, RacaId = srdCaninoId,
                Nome = "Bob", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2018, 9, 5, 0, 0, 0, DateTimeKind.Utc),
                Peso = 15.0m, Cor = "Caramelo", Castrado = true,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Paciente
            {
                Id = pac12, ClinicaId = clinica1Id, TutorId = tutorLucasId, EspecieId = felinoId, RacaId = srdFelinoId,
                Nome = "Mingau", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2023, 3, 10, 0, 0, 0, DateTimeKind.Utc),
                Peso = 3.8m, Cor = "Preto", Castrado = false,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            // Camila Souza - 2 pets
            new Paciente
            {
                Id = pac13, ClinicaId = clinica1Id, TutorId = tutorCamilaId, EspecieId = caninoId, RacaId = labradorId,
                Nome = "Max", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2021, 12, 25, 0, 0, 0, DateTimeKind.Utc),
                Peso = 30.0m, Cor = "Chocolate", Castrado = true, Alergias = "Corante alimentar",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Paciente
            {
                Id = pac14, ClinicaId = clinica1Id, TutorId = tutorCamilaId, EspecieId = aveId, RacaId = periquitoId,
                Nome = "Kiwi", Sexo = SexoPaciente.Femea, DataNascimento = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                Peso = 0.035m, Cor = "Verde e amarelo", Castrado = false,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            // Thiago Ribeiro - 2 pets
            new Paciente
            {
                Id = pac15, ClinicaId = clinica1Id, TutorId = tutorThiagoId, EspecieId = reptilId, RacaId = null,
                Nome = "Draco", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2022, 10, 30, 0, 0, 0, DateTimeKind.Utc),
                Peso = 0.45m, Cor = "Verde", Castrado = false, Observacoes = "Iguana verde, dócil",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Paciente
            {
                Id = pac16, ClinicaId = clinica1Id, TutorId = tutorThiagoId, EspecieId = caninoId, RacaId = goldenId,
                Nome = "Caramelo", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2020, 4, 18, 0, 0, 0, DateTimeKind.Utc),
                Peso = 33.0m, Cor = "Dourado", Castrado = true,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            // Patricia Lima - 2 pets
            new Paciente
            {
                Id = pac17, ClinicaId = clinica1Id, TutorId = tutorPatriciaId, EspecieId = felinoId, RacaId = persaId,
                Nome = "Neve", Sexo = SexoPaciente.Femea, DataNascimento = new DateTime(2021, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                Peso = 4.8m, Cor = "Branco", Castrado = true,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Paciente
            {
                Id = pac18, ClinicaId = clinica1Id, TutorId = tutorPatriciaId, EspecieId = roedorId, RacaId = porquinhoId,
                Nome = "Pipoca", Sexo = SexoPaciente.Femea, DataNascimento = new DateTime(2024, 3, 22, 0, 0, 0, DateTimeKind.Utc),
                Peso = 0.8m, Cor = "Tricolor", Castrado = false,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            // Bruno Carvalho - 2 pets
            new Paciente
            {
                Id = pac19, ClinicaId = clinica1Id, TutorId = tutorBrunoId, EspecieId = caninoId, RacaId = bulldogId,
                Nome = "Zeus", Sexo = SexoPaciente.Macho, DataNascimento = new DateTime(2022, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                Peso = 25.0m, Cor = "Tigrado", Castrado = false, Alergias = "Penicilina",
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Paciente
            {
                Id = pac20, ClinicaId = clinica1Id, TutorId = tutorBrunoId, EspecieId = felinoId, RacaId = siamesId,
                Nome = "Nina", Sexo = SexoPaciente.Femea, DataNascimento = new DateTime(2023, 11, 10, 0, 0, 0, DateTimeKind.Utc),
                Peso = 3.5m, Cor = "Creme e marrom", Castrado = true,
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            }
        );

        // ══════════════════════════════════════════════════════════════════
        // 11. APPOINTMENTS (8)
        // ══════════════════════════════════════════════════════════════════
        db.Agendamentos.AddRange(
            // Past - Concluido
            new Agendamento
            {
                Id = agend01, ClinicaId = clinica1Id, PacienteId = pac01, VeterinarioId = vetId,
                Tipo = TipoAgendamento.Consulta, Status = StatusAgendamento.Concluido,
                DataHoraAgendada = new DateTime(2026, 3, 20, 9, 0, 0, DateTimeKind.Utc), DuracaoMinutos = 30,
                IniciadoEm = new DateTime(2026, 3, 20, 9, 5, 0, DateTimeKind.Utc),
                FinalizadoEm = new DateTime(2026, 3, 20, 9, 30, 0, DateTimeKind.Utc),
                Motivo = "Check-up anual", CriadoEm = now, AtualizadoEm = now
            },
            new Agendamento
            {
                Id = agend02, ClinicaId = clinica1Id, PacienteId = pac03, VeterinarioId = vetId,
                Tipo = TipoAgendamento.Vacinacao, Status = StatusAgendamento.Concluido,
                DataHoraAgendada = new DateTime(2026, 3, 25, 10, 0, 0, DateTimeKind.Utc), DuracaoMinutos = 20,
                IniciadoEm = new DateTime(2026, 3, 25, 10, 0, 0, DateTimeKind.Utc),
                FinalizadoEm = new DateTime(2026, 3, 25, 10, 15, 0, DateTimeKind.Utc),
                Motivo = "Vacinação V10 reforço", CriadoEm = now, AtualizadoEm = now
            },
            new Agendamento
            {
                Id = agend03, ClinicaId = clinica1Id, PacienteId = pac05, VeterinarioId = vetId,
                Tipo = TipoAgendamento.Consulta, Status = StatusAgendamento.Concluido,
                DataHoraAgendada = new DateTime(2026, 4, 1, 14, 0, 0, DateTimeKind.Utc), DuracaoMinutos = 30,
                IniciadoEm = new DateTime(2026, 4, 1, 14, 0, 0, DateTimeKind.Utc),
                FinalizadoEm = new DateTime(2026, 4, 1, 14, 25, 0, DateTimeKind.Utc),
                Motivo = "Problema dermatológico", CriadoEm = now, AtualizadoEm = now
            },
            // Future - Agendado
            new Agendamento
            {
                Id = agend04, ClinicaId = clinica1Id, PacienteId = pac07, VeterinarioId = vetId,
                Tipo = TipoAgendamento.Consulta, Status = StatusAgendamento.Agendado,
                DataHoraAgendada = new DateTime(2026, 4, 15, 9, 0, 0, DateTimeKind.Utc), DuracaoMinutos = 30,
                Motivo = "Consulta de rotina", CriadoEm = now, AtualizadoEm = now
            },
            new Agendamento
            {
                Id = agend05, ClinicaId = clinica1Id, PacienteId = pac09, VeterinarioId = vetId,
                Tipo = TipoAgendamento.Vacinacao, Status = StatusAgendamento.Agendado,
                DataHoraAgendada = new DateTime(2026, 4, 16, 11, 0, 0, DateTimeKind.Utc), DuracaoMinutos = 20,
                Motivo = "Vacinação antirrábica", CriadoEm = now, AtualizadoEm = now
            },
            // Future - Confirmado
            new Agendamento
            {
                Id = agend06, ClinicaId = clinica1Id, PacienteId = pac13, VeterinarioId = vetId,
                Tipo = TipoAgendamento.BanhoTosa, Status = StatusAgendamento.Confirmado,
                DataHoraAgendada = new DateTime(2026, 4, 12, 8, 0, 0, DateTimeKind.Utc), DuracaoMinutos = 60,
                Motivo = "Banho e tosa higiênica", CriadoEm = now, AtualizadoEm = now
            },
            new Agendamento
            {
                Id = agend07, ClinicaId = clinica1Id, PacienteId = pac19, VeterinarioId = vetId,
                Tipo = TipoAgendamento.Retorno, Status = StatusAgendamento.Confirmado,
                DataHoraAgendada = new DateTime(2026, 4, 14, 15, 0, 0, DateTimeKind.Utc), DuracaoMinutos = 20,
                Motivo = "Retorno pós-tratamento dermatológico", CriadoEm = now, AtualizadoEm = now
            },
            new Agendamento
            {
                Id = agend08, ClinicaId = clinica1Id, PacienteId = pac16, VeterinarioId = vetId,
                Tipo = TipoAgendamento.Consulta, Status = StatusAgendamento.Agendado,
                DataHoraAgendada = new DateTime(2026, 4, 20, 10, 0, 0, DateTimeKind.Utc), DuracaoMinutos = 30,
                Motivo = "Dor na pata traseira", CriadoEm = now, AtualizadoEm = now
            }
        );

        // ══════════════════════════════════════════════════════════════════
        // 12. FINANCIAL CATEGORIES (6)
        // ══════════════════════════════════════════════════════════════════
        db.CategoriasFinanceiras.AddRange(
            new CategoriaFinanceira { Id = finCatConsultaId,  Nome = "Consulta",     Tipo = TipoTransacao.Receita,  Ativo = true, CriadoEm = now },
            new CategoriaFinanceira { Id = finCatCirurgiaId,  Nome = "Cirurgia",     Tipo = TipoTransacao.Receita,  Ativo = true, CriadoEm = now },
            new CategoriaFinanceira { Id = finCatVacinacaoId, Nome = "Vacinação",    Tipo = TipoTransacao.Receita,  Ativo = true, CriadoEm = now },
            new CategoriaFinanceira { Id = finCatMedicId,     Nome = "Medicamentos", Tipo = TipoTransacao.Receita,  Ativo = true, CriadoEm = now },
            new CategoriaFinanceira { Id = finCatAluguelId,   Nome = "Aluguel",      Tipo = TipoTransacao.Despesa,  Ativo = true, CriadoEm = now },
            new CategoriaFinanceira { Id = finCatSalariosId,  Nome = "Salários",     Tipo = TipoTransacao.Despesa,  Ativo = true, CriadoEm = now }
        );

        // ══════════════════════════════════════════════════════════════════
        // 13. PRODUCTS (5)
        // ══════════════════════════════════════════════════════════════════
        db.Produtos.AddRange(
            new Produto
            {
                Id = prodVacinaV10Id, ClinicaId = clinica1Id, CategoriaId = catVacinasId, UnidadeId = unidadeAmpId,
                Nome = "Vacina V10", Apresentacao = "Dose única 1ml",
                EstoqueAtual = 50, EstoqueMinimo = 10, PrecoCusto = 35.00m, PrecoVenda = 80.00m,
                Lote = "VV10-2026A", DataValidade = new DateTime(2027, 6, 30, 0, 0, 0, DateTimeKind.Utc),
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Produto
            {
                Id = prodNexgardId, ClinicaId = clinica1Id, CategoriaId = catMedicamentosId, UnidadeId = unidadeUnId,
                Nome = "Antipulgas Nexgard", Apresentacao = "Comprimido mastigável 10-25kg",
                EstoqueAtual = 30, EstoqueMinimo = 5, PrecoCusto = 45.00m, PrecoVenda = 95.00m,
                Lote = "NXG-2026B", DataValidade = new DateTime(2027, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Produto
            {
                Id = prodRoyalCaninId, ClinicaId = clinica1Id, CategoriaId = catMedicamentosId, UnidadeId = unidadeKgId,
                Nome = "Ração Royal Canin", Apresentacao = "Adulto raças médias 15kg",
                EstoqueAtual = 20, EstoqueMinimo = 3, PrecoCusto = 180.00m, PrecoVenda = 280.00m,
                Lote = "RC-MED-2026", DataValidade = new DateTime(2027, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Produto
            {
                Id = prodSeringaId, ClinicaId = clinica1Id, CategoriaId = catCirurgicosId, UnidadeId = unidadeCxId,
                Nome = "Seringa 5ml", Apresentacao = "Caixa com 100 unidades",
                EstoqueAtual = 15, EstoqueMinimo = 3, PrecoCusto = 25.00m, PrecoVenda = 45.00m,
                Lote = "SER5-2026", DataValidade = new DateTime(2028, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            },
            new Produto
            {
                Id = prodLuvaId, ClinicaId = clinica1Id, CategoriaId = catCirurgicosId, UnidadeId = unidadeCxId,
                Nome = "Luva Procedimento", Apresentacao = "Caixa com 100 unidades tamanho M",
                EstoqueAtual = 25, EstoqueMinimo = 5, PrecoCusto = 30.00m, PrecoVenda = 55.00m,
                Lote = "LUV-M-2026", DataValidade = new DateTime(2028, 6, 30, 0, 0, 0, DateTimeKind.Utc),
                Ativo = true, CriadoEm = now, AtualizadoEm = now
            }
        );

        await db.SaveChangesAsync();
    }
}
