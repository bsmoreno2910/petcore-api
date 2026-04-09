using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCore.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categoriasFinanceiras",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_categoriasFinanceiras", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categoriasProduto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    cor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_categoriasProduto", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clinicas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nomeFantasia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    razaoSocial = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    website = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    logoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    rua = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_clinicas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "especies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_especies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tiposExame",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    precoDefault = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_tiposExame", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "unidadesProduto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sigla = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_unidadesProduto", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    senhaHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    crmv = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    avatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "centrosCusto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_centrosCusto", x => x.id);
                    table.ForeignKey(
                        name: "FK_centrosCusto_clinicas_clinicaId",
                        column: x => x.clinicaId,
                        principalTable: "clinicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tutores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    rg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    telefoneSecundario = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    rua = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_tutores", x => x.id);
                    table.ForeignKey(
                        name: "FK_tutores_clinicas_clinicaId",
                        column: x => x.clinicaId,
                        principalTable: "clinicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "racas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    especieId = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_racas", x => x.id);
                    table.ForeignKey(
                        name: "FK_racas_especies_especieId",
                        column: x => x.especieId,
                        principalTable: "especies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "produtos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    categoriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    unidadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    apresentacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    estoqueAtual = table.Column<int>(type: "integer", nullable: false),
                    estoqueMinimo = table.Column<int>(type: "integer", nullable: false),
                    estoqueMaximo = table.Column<int>(type: "integer", nullable: true),
                    precoCusto = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    precoVenda = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    localizacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    codigoBarras = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    lote = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    dataValidade = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_produtos", x => x.id);
                    table.ForeignKey(
                        name: "FK_produtos_categoriasProduto_categoriaId",
                        column: x => x.categoriaId,
                        principalTable: "categoriasProduto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_produtos_clinicas_clinicaId",
                        column: x => x.clinicaId,
                        principalTable: "clinicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_produtos_unidadesProduto_unidadeId",
                        column: x => x.unidadeId,
                        principalTable: "unidadesProduto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "clinicaUsuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    usuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    perfil = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_clinicaUsuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_clinicaUsuarios_clinicas_clinicaId",
                        column: x => x.clinicaId,
                        principalTable: "clinicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_clinicaUsuarios_usuarios_usuarioId",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "logsAuditoria",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    usuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    acao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entidadeId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    valorAntigo = table.Column<string>(type: "text", nullable: true),
                    novoValor = table.Column<string>(type: "text", nullable: true),
                    enderecoIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_logsAuditoria", x => x.id);
                    table.ForeignKey(
                        name: "FK_logsAuditoria_usuarios_usuarioId",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pedidos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    periodo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    justificativa = table.Column<string>(type: "text", nullable: true),
                    criadoPorId = table.Column<Guid>(type: "uuid", nullable: false),
                    aprovadoPorId = table.Column<Guid>(type: "uuid", nullable: true),
                    dataAprovacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_pedidos", x => x.id);
                    table.ForeignKey(
                        name: "FK_pedidos_clinicas_clinicaId",
                        column: x => x.clinicaId,
                        principalTable: "clinicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedidos_usuarios_aprovadoPorId",
                        column: x => x.aprovadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_pedidos_usuarios_criadoPorId",
                        column: x => x.criadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pacientes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    tutorId = table.Column<Guid>(type: "uuid", nullable: false),
                    especieId = table.Column<Guid>(type: "uuid", nullable: false),
                    racaId = table.Column<Guid>(type: "uuid", nullable: true),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sexo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    dataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    peso = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    cor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    microchip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    castrado = table.Column<bool>(type: "boolean", nullable: false),
                    alergias = table.Column<string>(type: "text", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    fotoUrl = table.Column<string>(type: "text", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    obito = table.Column<bool>(type: "boolean", nullable: false),
                    dataObito = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_pacientes", x => x.id);
                    table.ForeignKey(
                        name: "FK_pacientes_clinicas_clinicaId",
                        column: x => x.clinicaId,
                        principalTable: "clinicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pacientes_especies_especieId",
                        column: x => x.especieId,
                        principalTable: "especies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pacientes_racas_racaId",
                        column: x => x.racaId,
                        principalTable: "racas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_pacientes_tutores_tutorId",
                        column: x => x.tutorId,
                        principalTable: "tutores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "itensPedido",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pedidoId = table.Column<Guid>(type: "uuid", nullable: false),
                    produtoId = table.Column<Guid>(type: "uuid", nullable: false),
                    quantidadeSolicitada = table.Column<int>(type: "integer", nullable: false),
                    quantidadeAprovada = table.Column<int>(type: "integer", nullable: true),
                    quantidadeRecebida = table.Column<int>(type: "integer", nullable: false),
                    observacoes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_itensPedido", x => x.id);
                    table.ForeignKey(
                        name: "FK_itensPedido_pedidos_pedidoId",
                        column: x => x.pedidoId,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_itensPedido_produtos_produtoId",
                        column: x => x.produtoId,
                        principalTable: "produtos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movimentacoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    produtoId = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    estoqueAnterior = table.Column<int>(type: "integer", nullable: false),
                    novoEstoque = table.Column<int>(type: "integer", nullable: false),
                    motivo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    criadoPorId = table.Column<Guid>(type: "uuid", nullable: false),
                    aprovadoPorId = table.Column<Guid>(type: "uuid", nullable: true),
                    dataAprovacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    pedidoId = table.Column<Guid>(type: "uuid", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_movimentacoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_movimentacoes_pedidos_pedidoId",
                        column: x => x.pedidoId,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_movimentacoes_produtos_produtoId",
                        column: x => x.produtoId,
                        principalTable: "produtos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movimentacoes_usuarios_aprovadoPorId",
                        column: x => x.aprovadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_movimentacoes_usuarios_criadoPorId",
                        column: x => x.criadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "agendamentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    pacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    veterinarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    dataHoraAgendada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    duracaoMinutos = table.Column<int>(type: "integer", nullable: false),
                    iniciadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    finalizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    motivoCancelamento = table.Column<string>(type: "text", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_agendamentos", x => x.id);
                    table.ForeignKey(
                        name: "FK_agendamentos_clinicas_clinicaId",
                        column: x => x.clinicaId,
                        principalTable: "clinicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_agendamentos_pacientes_pacienteId",
                        column: x => x.pacienteId,
                        principalTable: "pacientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_agendamentos_usuarios_veterinarioId",
                        column: x => x.veterinarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "internacoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    pacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    veterinarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    motivo = table.Column<string>(type: "text", nullable: true),
                    baia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    dieta = table.Column<string>(type: "text", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    dataInternacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dataAlta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    observacoesAlta = table.Column<string>(type: "text", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_internacoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_internacoes_pacientes_pacienteId",
                        column: x => x.pacienteId,
                        principalTable: "pacientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_internacoes_usuarios_veterinarioId",
                        column: x => x.veterinarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "prontuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    pacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    veterinarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    agendamentoId = table.Column<Guid>(type: "uuid", nullable: true),
                    queixaPrincipal = table.Column<string>(type: "text", nullable: true),
                    historico = table.Column<string>(type: "text", nullable: true),
                    anamnese = table.Column<string>(type: "text", nullable: true),
                    peso = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    temperatura = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    frequenciaCardiaca = table.Column<int>(type: "integer", nullable: true),
                    frequenciaRespiratoria = table.Column<int>(type: "integer", nullable: true),
                    exameFisico = table.Column<string>(type: "text", nullable: true),
                    mucosas = table.Column<string>(type: "text", nullable: true),
                    hidratacao = table.Column<string>(type: "text", nullable: true),
                    linfonodos = table.Column<string>(type: "text", nullable: true),
                    diagnostico = table.Column<string>(type: "text", nullable: true),
                    diagnosticoDiferencial = table.Column<string>(type: "text", nullable: true),
                    tratamento = table.Column<string>(type: "text", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    notasInternas = table.Column<string>(type: "text", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_prontuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_prontuarios_agendamentos_agendamentoId",
                        column: x => x.agendamentoId,
                        principalTable: "agendamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_prontuarios_pacientes_pacienteId",
                        column: x => x.pacienteId,
                        principalTable: "pacientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_prontuarios_usuarios_veterinarioId",
                        column: x => x.veterinarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evolucoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    internacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    veterinarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    peso = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    temperatura = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    frequenciaCardiaca = table.Column<int>(type: "integer", nullable: true),
                    frequenciaRespiratoria = table.Column<int>(type: "integer", nullable: true),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    medicamentos = table.Column<string>(type: "text", nullable: true),
                    alimentacao = table.Column<string>(type: "text", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_evolucoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_evolucoes_internacoes_internacaoId",
                        column: x => x.internacaoId,
                        principalTable: "internacoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_evolucoes_usuarios_veterinarioId",
                        column: x => x.veterinarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "prescricoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    prontuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    nomeMedicamento = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    dosagem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    frequencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    duracao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    viaAdministracao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    instrucoes = table.Column<string>(type: "text", nullable: true),
                    quantidade = table.Column<int>(type: "integer", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_prescricoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_prescricoes_prontuarios_prontuarioId",
                        column: x => x.prontuarioId,
                        principalTable: "prontuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solicitacoesExame",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    pacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    solicitadoPorId = table.Column<Guid>(type: "uuid", nullable: false),
                    tipoExameId = table.Column<Guid>(type: "uuid", nullable: false),
                    prontuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    indicacaoClinica = table.Column<string>(type: "text", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    dataSolicitacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dataColeta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dataConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_solicitacoesExame", x => x.id);
                    table.ForeignKey(
                        name: "FK_solicitacoesExame_pacientes_pacienteId",
                        column: x => x.pacienteId,
                        principalTable: "pacientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_solicitacoesExame_prontuarios_prontuarioId",
                        column: x => x.prontuarioId,
                        principalTable: "prontuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_solicitacoesExame_tiposExame_tipoExameId",
                        column: x => x.tipoExameId,
                        principalTable: "tiposExame",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_solicitacoesExame_usuarios_solicitadoPorId",
                        column: x => x.solicitadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "resultadosExame",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    solicitacaoExameId = table.Column<Guid>(type: "uuid", nullable: false),
                    realizadoPorId = table.Column<Guid>(type: "uuid", nullable: false),
                    textoResultado = table.Column<string>(type: "text", nullable: true),
                    arquivoResultadoUrl = table.Column<string>(type: "text", nullable: true),
                    valoresReferencia = table.Column<string>(type: "text", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    conclusao = table.Column<string>(type: "text", nullable: true),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_resultadosExame", x => x.id);
                    table.ForeignKey(
                        name: "FK_resultadosExame_solicitacoesExame_solicitacaoExameId",
                        column: x => x.solicitacaoExameId,
                        principalTable: "solicitacoesExame",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resultadosExame_usuarios_realizadoPorId",
                        column: x => x.realizadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transacoesFinanceiras",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    categoriaFinanceiraId = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    valor = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: false),
                    desconto = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: true),
                    valorPago = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: true),
                    metodoPagamento = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    dataVencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tutorId = table.Column<Guid>(type: "uuid", nullable: true),
                    agendamentoId = table.Column<Guid>(type: "uuid", nullable: true),
                    internacaoId = table.Column<Guid>(type: "uuid", nullable: true),
                    solicitacaoExameId = table.Column<Guid>(type: "uuid", nullable: true),
                    centroCustoId = table.Column<Guid>(type: "uuid", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    numeroNota = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    criadoPorId = table.Column<Guid>(type: "uuid", nullable: false),
                    criadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_transacoesFinanceiras", x => x.id);
                    table.ForeignKey(
                        name: "FK_transacoesFinanceiras_agendamentos_agendamentoId",
                        column: x => x.agendamentoId,
                        principalTable: "agendamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transacoesFinanceiras_categoriasFinanceiras_categoriaFinanc~",
                        column: x => x.categoriaFinanceiraId,
                        principalTable: "categoriasFinanceiras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transacoesFinanceiras_centrosCusto_centroCustoId",
                        column: x => x.centroCustoId,
                        principalTable: "centrosCusto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transacoesFinanceiras_clinicas_clinicaId",
                        column: x => x.clinicaId,
                        principalTable: "clinicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transacoesFinanceiras_internacoes_internacaoId",
                        column: x => x.internacaoId,
                        principalTable: "internacoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transacoesFinanceiras_solicitacoesExame_solicitacaoExameId",
                        column: x => x.solicitacaoExameId,
                        principalTable: "solicitacoesExame",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transacoesFinanceiras_tutores_tutorId",
                        column: x => x.tutorId,
                        principalTable: "tutores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transacoesFinanceiras_usuarios_criadoPorId",
                        column: x => x.criadoPorId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "parcelasTransacao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    numeroParcela = table.Column<int>(type: "integer", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: false),
                    dataVencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_parcelasTransacao", x => x.id);
                    table.ForeignKey(
                        name: "FK_parcelasTransacao_transacoesFinanceiras_transacaoId",
                        column: x => x.transacaoId,
                        principalTable: "transacoesFinanceiras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "iX_agendamentos_clinicaId_dataHoraAgendada",
                table: "agendamentos",
                columns: new[] { "clinicaId", "dataHoraAgendada" });

            migrationBuilder.CreateIndex(
                name: "iX_agendamentos_pacienteId",
                table: "agendamentos",
                column: "pacienteId");

            migrationBuilder.CreateIndex(
                name: "iX_agendamentos_veterinarioId",
                table: "agendamentos",
                column: "veterinarioId");

            migrationBuilder.CreateIndex(
                name: "iX_centrosCusto_clinicaId",
                table: "centrosCusto",
                column: "clinicaId");

            migrationBuilder.CreateIndex(
                name: "iX_clinicas_cnpj",
                table: "clinicas",
                column: "cnpj",
                unique: true,
                filter: "\"cnpj\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "iX_clinicaUsuarios_clinicaId_usuarioId",
                table: "clinicaUsuarios",
                columns: new[] { "clinicaId", "usuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "iX_clinicaUsuarios_usuarioId",
                table: "clinicaUsuarios",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "iX_especies_nome",
                table: "especies",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "iX_evolucoes_internacaoId",
                table: "evolucoes",
                column: "internacaoId");

            migrationBuilder.CreateIndex(
                name: "iX_evolucoes_veterinarioId",
                table: "evolucoes",
                column: "veterinarioId");

            migrationBuilder.CreateIndex(
                name: "iX_internacoes_pacienteId",
                table: "internacoes",
                column: "pacienteId");

            migrationBuilder.CreateIndex(
                name: "iX_internacoes_veterinarioId",
                table: "internacoes",
                column: "veterinarioId");

            migrationBuilder.CreateIndex(
                name: "iX_itensPedido_pedidoId",
                table: "itensPedido",
                column: "pedidoId");

            migrationBuilder.CreateIndex(
                name: "iX_itensPedido_produtoId",
                table: "itensPedido",
                column: "produtoId");

            migrationBuilder.CreateIndex(
                name: "iX_logsAuditoria_clinicaId_criadoEm",
                table: "logsAuditoria",
                columns: new[] { "clinicaId", "criadoEm" });

            migrationBuilder.CreateIndex(
                name: "iX_logsAuditoria_entidade",
                table: "logsAuditoria",
                column: "entidade");

            migrationBuilder.CreateIndex(
                name: "iX_logsAuditoria_usuarioId",
                table: "logsAuditoria",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "iX_movimentacoes_aprovadoPorId",
                table: "movimentacoes",
                column: "aprovadoPorId");

            migrationBuilder.CreateIndex(
                name: "iX_movimentacoes_criadoPorId",
                table: "movimentacoes",
                column: "criadoPorId");

            migrationBuilder.CreateIndex(
                name: "iX_movimentacoes_pedidoId",
                table: "movimentacoes",
                column: "pedidoId");

            migrationBuilder.CreateIndex(
                name: "iX_movimentacoes_produtoId",
                table: "movimentacoes",
                column: "produtoId");

            migrationBuilder.CreateIndex(
                name: "iX_pacientes_clinicaId_nome",
                table: "pacientes",
                columns: new[] { "clinicaId", "nome" });

            migrationBuilder.CreateIndex(
                name: "iX_pacientes_especieId",
                table: "pacientes",
                column: "especieId");

            migrationBuilder.CreateIndex(
                name: "iX_pacientes_racaId",
                table: "pacientes",
                column: "racaId");

            migrationBuilder.CreateIndex(
                name: "iX_pacientes_tutorId",
                table: "pacientes",
                column: "tutorId");

            migrationBuilder.CreateIndex(
                name: "iX_parcelasTransacao_transacaoId",
                table: "parcelasTransacao",
                column: "transacaoId");

            migrationBuilder.CreateIndex(
                name: "iX_pedidos_aprovadoPorId",
                table: "pedidos",
                column: "aprovadoPorId");

            migrationBuilder.CreateIndex(
                name: "iX_pedidos_clinicaId",
                table: "pedidos",
                column: "clinicaId");

            migrationBuilder.CreateIndex(
                name: "iX_pedidos_codigo",
                table: "pedidos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "iX_pedidos_criadoPorId",
                table: "pedidos",
                column: "criadoPorId");

            migrationBuilder.CreateIndex(
                name: "iX_prescricoes_prontuarioId",
                table: "prescricoes",
                column: "prontuarioId");

            migrationBuilder.CreateIndex(
                name: "iX_produtos_categoriaId",
                table: "produtos",
                column: "categoriaId");

            migrationBuilder.CreateIndex(
                name: "iX_produtos_clinicaId_codigoBarras",
                table: "produtos",
                columns: new[] { "clinicaId", "codigoBarras" },
                unique: true,
                filter: "\"codigoBarras\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "iX_produtos_clinicaId_nome",
                table: "produtos",
                columns: new[] { "clinicaId", "nome" });

            migrationBuilder.CreateIndex(
                name: "iX_produtos_unidadeId",
                table: "produtos",
                column: "unidadeId");

            migrationBuilder.CreateIndex(
                name: "iX_prontuarios_agendamentoId",
                table: "prontuarios",
                column: "agendamentoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "iX_prontuarios_clinicaId_criadoEm",
                table: "prontuarios",
                columns: new[] { "clinicaId", "criadoEm" });

            migrationBuilder.CreateIndex(
                name: "iX_prontuarios_pacienteId",
                table: "prontuarios",
                column: "pacienteId");

            migrationBuilder.CreateIndex(
                name: "iX_prontuarios_veterinarioId",
                table: "prontuarios",
                column: "veterinarioId");

            migrationBuilder.CreateIndex(
                name: "iX_racas_especieId",
                table: "racas",
                column: "especieId");

            migrationBuilder.CreateIndex(
                name: "iX_resultadosExame_realizadoPorId",
                table: "resultadosExame",
                column: "realizadoPorId");

            migrationBuilder.CreateIndex(
                name: "iX_resultadosExame_solicitacaoExameId",
                table: "resultadosExame",
                column: "solicitacaoExameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "iX_solicitacoesExame_pacienteId",
                table: "solicitacoesExame",
                column: "pacienteId");

            migrationBuilder.CreateIndex(
                name: "iX_solicitacoesExame_prontuarioId",
                table: "solicitacoesExame",
                column: "prontuarioId");

            migrationBuilder.CreateIndex(
                name: "iX_solicitacoesExame_solicitadoPorId",
                table: "solicitacoesExame",
                column: "solicitadoPorId");

            migrationBuilder.CreateIndex(
                name: "iX_solicitacoesExame_tipoExameId",
                table: "solicitacoesExame",
                column: "tipoExameId");

            migrationBuilder.CreateIndex(
                name: "iX_transacoesFinanceiras_agendamentoId",
                table: "transacoesFinanceiras",
                column: "agendamentoId");

            migrationBuilder.CreateIndex(
                name: "iX_transacoesFinanceiras_categoriaFinanceiraId",
                table: "transacoesFinanceiras",
                column: "categoriaFinanceiraId");

            migrationBuilder.CreateIndex(
                name: "iX_transacoesFinanceiras_centroCustoId",
                table: "transacoesFinanceiras",
                column: "centroCustoId");

            migrationBuilder.CreateIndex(
                name: "iX_transacoesFinanceiras_clinicaId_dataVencimento",
                table: "transacoesFinanceiras",
                columns: new[] { "clinicaId", "dataVencimento" });

            migrationBuilder.CreateIndex(
                name: "iX_transacoesFinanceiras_clinicaId_status",
                table: "transacoesFinanceiras",
                columns: new[] { "clinicaId", "status" });

            migrationBuilder.CreateIndex(
                name: "iX_transacoesFinanceiras_criadoPorId",
                table: "transacoesFinanceiras",
                column: "criadoPorId");

            migrationBuilder.CreateIndex(
                name: "iX_transacoesFinanceiras_internacaoId",
                table: "transacoesFinanceiras",
                column: "internacaoId");

            migrationBuilder.CreateIndex(
                name: "iX_transacoesFinanceiras_solicitacaoExameId",
                table: "transacoesFinanceiras",
                column: "solicitacaoExameId");

            migrationBuilder.CreateIndex(
                name: "iX_transacoesFinanceiras_tutorId",
                table: "transacoesFinanceiras",
                column: "tutorId");

            migrationBuilder.CreateIndex(
                name: "iX_tutores_clinicaId_cpf",
                table: "tutores",
                columns: new[] { "clinicaId", "cpf" },
                unique: true,
                filter: "\"cpf\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "iX_tutores_clinicaId_email",
                table: "tutores",
                columns: new[] { "clinicaId", "email" },
                unique: true,
                filter: "\"email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "iX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clinicaUsuarios");

            migrationBuilder.DropTable(
                name: "evolucoes");

            migrationBuilder.DropTable(
                name: "itensPedido");

            migrationBuilder.DropTable(
                name: "logsAuditoria");

            migrationBuilder.DropTable(
                name: "movimentacoes");

            migrationBuilder.DropTable(
                name: "parcelasTransacao");

            migrationBuilder.DropTable(
                name: "prescricoes");

            migrationBuilder.DropTable(
                name: "resultadosExame");

            migrationBuilder.DropTable(
                name: "pedidos");

            migrationBuilder.DropTable(
                name: "produtos");

            migrationBuilder.DropTable(
                name: "transacoesFinanceiras");

            migrationBuilder.DropTable(
                name: "categoriasProduto");

            migrationBuilder.DropTable(
                name: "unidadesProduto");

            migrationBuilder.DropTable(
                name: "categoriasFinanceiras");

            migrationBuilder.DropTable(
                name: "centrosCusto");

            migrationBuilder.DropTable(
                name: "internacoes");

            migrationBuilder.DropTable(
                name: "solicitacoesExame");

            migrationBuilder.DropTable(
                name: "prontuarios");

            migrationBuilder.DropTable(
                name: "tiposExame");

            migrationBuilder.DropTable(
                name: "agendamentos");

            migrationBuilder.DropTable(
                name: "pacientes");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "racas");

            migrationBuilder.DropTable(
                name: "tutores");

            migrationBuilder.DropTable(
                name: "especies");

            migrationBuilder.DropTable(
                name: "clinicas");
        }
    }
}
