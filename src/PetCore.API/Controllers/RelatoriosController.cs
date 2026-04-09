using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize(Roles = "SuperAdmin,Admin,Financeiro,Visualizador")]
public class RelatoriosController : ControllerBase
{
    private readonly AppDbContext _db;
    public RelatoriosController(AppDbContext db) { _db = db; }

    private Guid ClinicaId => (Guid)HttpContext.Items["ClinicaId"]!;

    private const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private static byte[] GerarExcel(Action<IXLWorksheet> preencher, string nomePlanilha = "Dados")
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(nomePlanilha);
        preencher(ws);
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    // 1. GET /api/relatorios/inventario
    [HttpGet("inventario")]
    public async Task<IActionResult> Inventario()
    {
        var dados = await _db.Produtos
            .Where(p => p.ClinicaId == ClinicaId)
            .Include(p => p.Categoria)
            .Include(p => p.Unidade)
            .OrderBy(p => p.Nome)
            .Select(p => new
            {
                p.Nome,
                Categoria = p.Categoria.Nome,
                Unidade = p.Unidade.Nome,
                p.Apresentacao,
                p.EstoqueAtual,
                p.EstoqueMinimo,
                p.EstoqueMaximo,
                p.PrecoCusto,
                p.PrecoVenda,
                p.Localizacao,
                p.CodigoBarras,
                p.Lote,
                p.DataValidade,
                p.Ativo
            })
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            ws.Cell(1, 1).Value = "Nome";
            ws.Cell(1, 2).Value = "Categoria";
            ws.Cell(1, 3).Value = "Unidade";
            ws.Cell(1, 4).Value = "Apresentacao";
            ws.Cell(1, 5).Value = "Estoque Atual";
            ws.Cell(1, 6).Value = "Estoque Minimo";
            ws.Cell(1, 7).Value = "Estoque Maximo";
            ws.Cell(1, 8).Value = "Preco Custo";
            ws.Cell(1, 9).Value = "Preco Venda";
            ws.Cell(1, 10).Value = "Localizacao";
            ws.Cell(1, 11).Value = "Codigo Barras";
            ws.Cell(1, 12).Value = "Lote";
            ws.Cell(1, 13).Value = "Data Validade";
            ws.Cell(1, 14).Value = "Ativo";
            ws.Range("A1:N1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].Nome;
                ws.Cell(r, 2).Value = dados[i].Categoria;
                ws.Cell(r, 3).Value = dados[i].Unidade;
                ws.Cell(r, 4).Value = dados[i].Apresentacao;
                ws.Cell(r, 5).Value = dados[i].EstoqueAtual;
                ws.Cell(r, 6).Value = dados[i].EstoqueMinimo;
                ws.Cell(r, 7).Value = dados[i].EstoqueMaximo ?? 0;
                ws.Cell(r, 8).Value = (double)(dados[i].PrecoCusto ?? 0);
                ws.Cell(r, 9).Value = (double)(dados[i].PrecoVenda ?? 0);
                ws.Cell(r, 10).Value = dados[i].Localizacao;
                ws.Cell(r, 11).Value = dados[i].CodigoBarras;
                ws.Cell(r, 12).Value = dados[i].Lote;
                ws.Cell(r, 13).Value = dados[i].DataValidade?.ToString("dd/MM/yyyy");
                ws.Cell(r, 14).Value = dados[i].Ativo ? "Sim" : "Nao";
            }
        }, "Inventario");

        return File(bytes, ContentType, "PetCore_Inventario.xlsx");
    }

    // 2. GET /api/relatorios/movimentacoes
    [HttpGet("movimentacoes")]
    public async Task<IActionResult> Movimentacoes([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var query = _db.Movimentacoes
            .Where(m => m.ClinicaId == ClinicaId)
            .Include(m => m.Produto)
            .Include(m => m.CriadoPor)
            .AsQueryable();

        if (dataInicio.HasValue) query = query.Where(m => m.CriadoEm >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(m => m.CriadoEm <= dataFim.Value);

        var dados = await query.OrderByDescending(m => m.CriadoEm)
            .Select(m => new
            {
                m.CriadoEm,
                Produto = m.Produto.Nome,
                Tipo = m.Tipo.ToString(),
                m.Quantidade,
                m.EstoqueAnterior,
                m.NovoEstoque,
                m.Motivo,
                CriadoPor = m.CriadoPor.Nome,
                m.Observacoes
            })
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Data", "Produto", "Tipo", "Quantidade", "Estoque Anterior", "Novo Estoque", "Motivo", "Criado Por", "Observacoes"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:I1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].CriadoEm.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(r, 2).Value = dados[i].Produto;
                ws.Cell(r, 3).Value = dados[i].Tipo;
                ws.Cell(r, 4).Value = dados[i].Quantidade;
                ws.Cell(r, 5).Value = dados[i].EstoqueAnterior;
                ws.Cell(r, 6).Value = dados[i].NovoEstoque;
                ws.Cell(r, 7).Value = dados[i].Motivo;
                ws.Cell(r, 8).Value = dados[i].CriadoPor;
                ws.Cell(r, 9).Value = dados[i].Observacoes;
            }
        }, "Movimentacoes");

        return File(bytes, ContentType, "PetCore_Movimentacoes.xlsx");
    }

    // 3. GET /api/relatorios/agendamentos
    [HttpGet("agendamentos")]
    public async Task<IActionResult> Agendamentos([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var query = _db.Agendamentos
            .Where(a => a.ClinicaId == ClinicaId)
            .Include(a => a.Paciente).ThenInclude(p => p.Tutor)
            .Include(a => a.Veterinario)
            .AsQueryable();

        if (dataInicio.HasValue) query = query.Where(a => a.DataHoraAgendada >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(a => a.DataHoraAgendada <= dataFim.Value);

        var dados = await query.OrderByDescending(a => a.DataHoraAgendada)
            .Select(a => new
            {
                a.DataHoraAgendada,
                Paciente = a.Paciente.Nome,
                Tutor = a.Paciente.Tutor.Nome,
                Veterinario = a.Veterinario != null ? a.Veterinario.Nome : "",
                Tipo = a.Tipo.ToString(),
                Status = a.Status.ToString(),
                a.DuracaoMinutos,
                a.Motivo,
                a.Observacoes
            })
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Data/Hora", "Paciente", "Tutor", "Veterinario", "Tipo", "Status", "Duracao (min)", "Motivo", "Observacoes"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:I1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].DataHoraAgendada.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(r, 2).Value = dados[i].Paciente;
                ws.Cell(r, 3).Value = dados[i].Tutor;
                ws.Cell(r, 4).Value = dados[i].Veterinario;
                ws.Cell(r, 5).Value = dados[i].Tipo;
                ws.Cell(r, 6).Value = dados[i].Status;
                ws.Cell(r, 7).Value = dados[i].DuracaoMinutos;
                ws.Cell(r, 8).Value = dados[i].Motivo;
                ws.Cell(r, 9).Value = dados[i].Observacoes;
            }
        }, "Agendamentos");

        return File(bytes, ContentType, "PetCore_Agendamentos.xlsx");
    }

    // 4. GET /api/relatorios/pacientes
    [HttpGet("pacientes")]
    public async Task<IActionResult> Pacientes()
    {
        var dados = await _db.Pacientes
            .Where(p => p.ClinicaId == ClinicaId)
            .Include(p => p.Tutor)
            .Include(p => p.Especie)
            .Include(p => p.Raca)
            .OrderBy(p => p.Nome)
            .Select(p => new
            {
                p.Nome,
                Tutor = p.Tutor.Nome,
                TutorTelefone = p.Tutor.Telefone,
                Especie = p.Especie.Nome,
                Raca = p.Raca != null ? p.Raca.Nome : "",
                Sexo = p.Sexo.ToString(),
                p.DataNascimento,
                p.Peso,
                p.Cor,
                p.Microchip,
                p.Castrado,
                p.Ativo,
                p.Obito
            })
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Nome", "Tutor", "Telefone Tutor", "Especie", "Raca", "Sexo", "Data Nascimento", "Peso", "Cor", "Microchip", "Castrado", "Ativo", "Obito"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:M1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].Nome;
                ws.Cell(r, 2).Value = dados[i].Tutor;
                ws.Cell(r, 3).Value = dados[i].TutorTelefone;
                ws.Cell(r, 4).Value = dados[i].Especie;
                ws.Cell(r, 5).Value = dados[i].Raca;
                ws.Cell(r, 6).Value = dados[i].Sexo;
                ws.Cell(r, 7).Value = dados[i].DataNascimento?.ToString("dd/MM/yyyy");
                ws.Cell(r, 8).Value = (double)(dados[i].Peso ?? 0);
                ws.Cell(r, 9).Value = dados[i].Cor;
                ws.Cell(r, 10).Value = dados[i].Microchip;
                ws.Cell(r, 11).Value = dados[i].Castrado ? "Sim" : "Nao";
                ws.Cell(r, 12).Value = dados[i].Ativo ? "Sim" : "Nao";
                ws.Cell(r, 13).Value = dados[i].Obito ? "Sim" : "Nao";
            }
        }, "Pacientes");

        return File(bytes, ContentType, "PetCore_Pacientes.xlsx");
    }

    // 5. GET /api/relatorios/tutores
    [HttpGet("tutores")]
    public async Task<IActionResult> Tutores()
    {
        var dados = await _db.Tutores
            .Where(t => t.ClinicaId == ClinicaId)
            .OrderBy(t => t.Nome)
            .Select(t => new
            {
                t.Nome,
                t.Cpf,
                t.Telefone,
                t.TelefoneSecundario,
                t.Email,
                t.Cidade,
                t.Estado,
                t.Bairro,
                t.Rua,
                t.Numero,
                t.Cep,
                QuantidadePacientes = t.Pacientes.Count,
                t.Ativo,
                t.CriadoEm
            })
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Nome", "CPF", "Telefone", "Telefone Secundario", "Email", "Cidade", "Estado", "Bairro", "Rua", "Numero", "CEP", "Qtd Pacientes", "Ativo", "Criado Em"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:N1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].Nome;
                ws.Cell(r, 2).Value = dados[i].Cpf;
                ws.Cell(r, 3).Value = dados[i].Telefone;
                ws.Cell(r, 4).Value = dados[i].TelefoneSecundario;
                ws.Cell(r, 5).Value = dados[i].Email;
                ws.Cell(r, 6).Value = dados[i].Cidade;
                ws.Cell(r, 7).Value = dados[i].Estado;
                ws.Cell(r, 8).Value = dados[i].Bairro;
                ws.Cell(r, 9).Value = dados[i].Rua;
                ws.Cell(r, 10).Value = dados[i].Numero;
                ws.Cell(r, 11).Value = dados[i].Cep;
                ws.Cell(r, 12).Value = dados[i].QuantidadePacientes;
                ws.Cell(r, 13).Value = dados[i].Ativo ? "Sim" : "Nao";
                ws.Cell(r, 14).Value = dados[i].CriadoEm.ToString("dd/MM/yyyy");
            }
        }, "Tutores");

        return File(bytes, ContentType, "PetCore_Tutores.xlsx");
    }

    // 6. GET /api/relatorios/internacoes
    [HttpGet("internacoes")]
    public async Task<IActionResult> Internacoes([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var query = _db.Internacoes
            .Where(i => i.ClinicaId == ClinicaId)
            .Include(i => i.Paciente).ThenInclude(p => p.Tutor)
            .Include(i => i.Veterinario)
            .AsQueryable();

        if (dataInicio.HasValue) query = query.Where(i => i.DataInternacao >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(i => i.DataInternacao <= dataFim.Value);

        var dados = await query.OrderByDescending(i => i.DataInternacao)
            .Select(i => new
            {
                i.DataInternacao,
                i.DataAlta,
                Paciente = i.Paciente.Nome,
                Tutor = i.Paciente.Tutor.Nome,
                Veterinario = i.Veterinario.Nome,
                Status = i.Status.ToString(),
                i.Motivo,
                i.Baia,
                i.Dieta,
                i.Observacoes,
                i.ObservacoesAlta
            })
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Data Internacao", "Data Alta", "Paciente", "Tutor", "Veterinario", "Status", "Motivo", "Baia", "Dieta", "Observacoes", "Obs. Alta"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:K1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].DataInternacao.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(r, 2).Value = dados[i].DataAlta?.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(r, 3).Value = dados[i].Paciente;
                ws.Cell(r, 4).Value = dados[i].Tutor;
                ws.Cell(r, 5).Value = dados[i].Veterinario;
                ws.Cell(r, 6).Value = dados[i].Status;
                ws.Cell(r, 7).Value = dados[i].Motivo;
                ws.Cell(r, 8).Value = dados[i].Baia;
                ws.Cell(r, 9).Value = dados[i].Dieta;
                ws.Cell(r, 10).Value = dados[i].Observacoes;
                ws.Cell(r, 11).Value = dados[i].ObservacoesAlta;
            }
        }, "Internacoes");

        return File(bytes, ContentType, "PetCore_Internacoes.xlsx");
    }

    // 7. GET /api/relatorios/exames
    [HttpGet("exames")]
    public async Task<IActionResult> Exames([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var query = _db.SolicitacoesExame
            .Where(e => e.ClinicaId == ClinicaId)
            .Include(e => e.Paciente).ThenInclude(p => p.Tutor)
            .Include(e => e.SolicitadoPor)
            .Include(e => e.TipoExame)
            .AsQueryable();

        if (dataInicio.HasValue) query = query.Where(e => e.DataSolicitacao >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(e => e.DataSolicitacao <= dataFim.Value);

        var dados = await query.OrderByDescending(e => e.DataSolicitacao)
            .Select(e => new
            {
                e.DataSolicitacao,
                e.DataColeta,
                e.DataConclusao,
                Paciente = e.Paciente.Nome,
                Tutor = e.Paciente.Tutor.Nome,
                SolicitadoPor = e.SolicitadoPor.Nome,
                TipoExame = e.TipoExame.Nome,
                Status = e.Status.ToString(),
                e.IndicacaoClinica,
                e.Observacoes
            })
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Data Solicitacao", "Data Coleta", "Data Conclusao", "Paciente", "Tutor", "Solicitado Por", "Tipo Exame", "Status", "Indicacao Clinica", "Observacoes"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:J1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].DataSolicitacao.ToString("dd/MM/yyyy");
                ws.Cell(r, 2).Value = dados[i].DataColeta?.ToString("dd/MM/yyyy");
                ws.Cell(r, 3).Value = dados[i].DataConclusao?.ToString("dd/MM/yyyy");
                ws.Cell(r, 4).Value = dados[i].Paciente;
                ws.Cell(r, 5).Value = dados[i].Tutor;
                ws.Cell(r, 6).Value = dados[i].SolicitadoPor;
                ws.Cell(r, 7).Value = dados[i].TipoExame;
                ws.Cell(r, 8).Value = dados[i].Status;
                ws.Cell(r, 9).Value = dados[i].IndicacaoClinica;
                ws.Cell(r, 10).Value = dados[i].Observacoes;
            }
        }, "Exames");

        return File(bytes, ContentType, "PetCore_Exames.xlsx");
    }

    // 8. GET /api/relatorios/receitas
    [HttpGet("receitas")]
    public async Task<IActionResult> Receitas([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var bytes = await GerarRelatorioFinanceiro(TipoTransacao.Receita, dataInicio, dataFim);
        return File(bytes, ContentType, "PetCore_Receitas.xlsx");
    }

    // 9. GET /api/relatorios/despesas
    [HttpGet("despesas")]
    public async Task<IActionResult> Despesas([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var bytes = await GerarRelatorioFinanceiro(TipoTransacao.Despesa, dataInicio, dataFim);
        return File(bytes, ContentType, "PetCore_Despesas.xlsx");
    }

    private async Task<byte[]> GerarRelatorioFinanceiro(TipoTransacao tipo, DateTime? dataInicio, DateTime? dataFim)
    {
        var query = _db.TransacoesFinanceiras
            .Where(t => t.ClinicaId == ClinicaId && t.Tipo == tipo)
            .Include(t => t.CategoriaFinanceira)
            .Include(t => t.Tutor)
            .Include(t => t.CentroCusto)
            .AsQueryable();

        if (dataInicio.HasValue) query = query.Where(t => t.DataVencimento >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(t => t.DataVencimento <= dataFim.Value);

        var dados = await query.OrderByDescending(t => t.DataVencimento)
            .Select(t => new
            {
                t.Descricao,
                Categoria = t.CategoriaFinanceira.Nome,
                Tutor = t.Tutor != null ? t.Tutor.Nome : "",
                CentroCusto = t.CentroCusto != null ? t.CentroCusto.Nome : "",
                t.Valor,
                t.Desconto,
                t.ValorPago,
                Status = t.Status.ToString(),
                MetodoPagamento = t.MetodoPagamento != null ? t.MetodoPagamento.ToString() : "",
                t.DataVencimento,
                t.DataPagamento,
                t.NumeroNota,
                t.Observacoes
            })
            .ToListAsync();

        var nomePlanilha = tipo == TipoTransacao.Receita ? "Receitas" : "Despesas";

        return GerarExcel(ws =>
        {
            string[] headers = ["Descricao", "Categoria", "Tutor", "Centro de Custo", "Valor", "Desconto", "Valor Pago", "Status", "Metodo Pagamento", "Data Vencimento", "Data Pagamento", "Numero Nota", "Observacoes"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:M1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].Descricao;
                ws.Cell(r, 2).Value = dados[i].Categoria;
                ws.Cell(r, 3).Value = dados[i].Tutor;
                ws.Cell(r, 4).Value = dados[i].CentroCusto;
                ws.Cell(r, 5).Value = (double)dados[i].Valor;
                ws.Cell(r, 6).Value = (double)(dados[i].Desconto ?? 0);
                ws.Cell(r, 7).Value = (double)(dados[i].ValorPago ?? 0);
                ws.Cell(r, 8).Value = dados[i].Status;
                ws.Cell(r, 9).Value = dados[i].MetodoPagamento;
                ws.Cell(r, 10).Value = dados[i].DataVencimento.ToString("dd/MM/yyyy");
                ws.Cell(r, 11).Value = dados[i].DataPagamento?.ToString("dd/MM/yyyy");
                ws.Cell(r, 12).Value = dados[i].NumeroNota;
                ws.Cell(r, 13).Value = dados[i].Observacoes;
            }
        }, nomePlanilha);
    }

    // 10. GET /api/relatorios/fluxo-caixa
    [HttpGet("fluxo-caixa")]
    public async Task<IActionResult> FluxoCaixa([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var query = _db.TransacoesFinanceiras
            .Where(t => t.ClinicaId == ClinicaId)
            .Include(t => t.CategoriaFinanceira)
            .AsQueryable();

        if (dataInicio.HasValue) query = query.Where(t => t.DataVencimento >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(t => t.DataVencimento <= dataFim.Value);

        var dados = await query.OrderBy(t => t.DataVencimento)
            .Select(t => new
            {
                t.DataVencimento,
                t.DataPagamento,
                t.Descricao,
                Tipo = t.Tipo.ToString(),
                Categoria = t.CategoriaFinanceira.Nome,
                t.Valor,
                t.ValorPago,
                Status = t.Status.ToString()
            })
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Data Vencimento", "Data Pagamento", "Descricao", "Tipo", "Categoria", "Valor", "Valor Pago", "Status"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:H1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].DataVencimento.ToString("dd/MM/yyyy");
                ws.Cell(r, 2).Value = dados[i].DataPagamento?.ToString("dd/MM/yyyy");
                ws.Cell(r, 3).Value = dados[i].Descricao;
                ws.Cell(r, 4).Value = dados[i].Tipo;
                ws.Cell(r, 5).Value = dados[i].Categoria;
                ws.Cell(r, 6).Value = (double)dados[i].Valor;
                ws.Cell(r, 7).Value = (double)(dados[i].ValorPago ?? 0);
                ws.Cell(r, 8).Value = dados[i].Status;
            }

            // Summary row
            var totalRow = dados.Count + 3;
            ws.Cell(totalRow, 5).Value = "Total Receitas:";
            ws.Cell(totalRow, 5).Style.Font.Bold = true;
            ws.Cell(totalRow, 6).Value = (double)dados.Where(d => d.Tipo == "Receita").Sum(d => d.Valor);
            ws.Cell(totalRow + 1, 5).Value = "Total Despesas:";
            ws.Cell(totalRow + 1, 5).Style.Font.Bold = true;
            ws.Cell(totalRow + 1, 6).Value = (double)dados.Where(d => d.Tipo == "Despesa").Sum(d => d.Valor);
            ws.Cell(totalRow + 2, 5).Value = "Saldo:";
            ws.Cell(totalRow + 2, 5).Style.Font.Bold = true;
            ws.Cell(totalRow + 2, 6).Value = (double)(dados.Where(d => d.Tipo == "Receita").Sum(d => d.Valor) - dados.Where(d => d.Tipo == "Despesa").Sum(d => d.Valor));
        }, "Fluxo de Caixa");

        return File(bytes, ContentType, "PetCore_FluxoCaixa.xlsx");
    }

    // 11. GET /api/relatorios/inadimplencia
    [HttpGet("inadimplencia")]
    public async Task<IActionResult> Inadimplencia()
    {
        var dados = await _db.TransacoesFinanceiras
            .Where(t => t.ClinicaId == ClinicaId && t.Status == StatusTransacao.Atrasado)
            .Include(t => t.CategoriaFinanceira)
            .Include(t => t.Tutor)
            .OrderBy(t => t.DataVencimento)
            .Select(t => new
            {
                t.Descricao,
                Categoria = t.CategoriaFinanceira.Nome,
                Tutor = t.Tutor != null ? t.Tutor.Nome : "",
                TutorTelefone = t.Tutor != null ? t.Tutor.Telefone : "",
                t.Valor,
                t.ValorPago,
                t.DataVencimento,
                DiasAtraso = (DateTime.UtcNow - t.DataVencimento).Days
            })
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Descricao", "Categoria", "Tutor", "Telefone Tutor", "Valor", "Valor Pago", "Data Vencimento", "Dias em Atraso"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:H1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].Descricao;
                ws.Cell(r, 2).Value = dados[i].Categoria;
                ws.Cell(r, 3).Value = dados[i].Tutor;
                ws.Cell(r, 4).Value = dados[i].TutorTelefone;
                ws.Cell(r, 5).Value = (double)dados[i].Valor;
                ws.Cell(r, 6).Value = (double)(dados[i].ValorPago ?? 0);
                ws.Cell(r, 7).Value = dados[i].DataVencimento.ToString("dd/MM/yyyy");
                ws.Cell(r, 8).Value = dados[i].DiasAtraso;
            }
        }, "Inadimplencia");

        return File(bytes, ContentType, "PetCore_Inadimplencia.xlsx");
    }

    // 12. GET /api/relatorios/por-categoria
    [HttpGet("por-categoria")]
    public async Task<IActionResult> PorCategoria([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var query = _db.TransacoesFinanceiras
            .Where(t => t.ClinicaId == ClinicaId)
            .Include(t => t.CategoriaFinanceira)
            .AsQueryable();

        if (dataInicio.HasValue) query = query.Where(t => t.DataVencimento >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(t => t.DataVencimento <= dataFim.Value);

        var dados = await query
            .GroupBy(t => new { t.CategoriaFinanceira.Nome, Tipo = t.Tipo })
            .Select(g => new
            {
                Categoria = g.Key.Nome,
                Tipo = g.Key.Tipo.ToString(),
                Quantidade = g.Count(),
                ValorTotal = g.Sum(t => t.Valor),
                ValorPago = g.Sum(t => t.ValorPago ?? 0)
            })
            .OrderBy(x => x.Tipo)
            .ThenByDescending(x => x.ValorTotal)
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Categoria", "Tipo", "Quantidade", "Valor Total", "Valor Pago"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:E1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].Categoria;
                ws.Cell(r, 2).Value = dados[i].Tipo;
                ws.Cell(r, 3).Value = dados[i].Quantidade;
                ws.Cell(r, 4).Value = (double)dados[i].ValorTotal;
                ws.Cell(r, 5).Value = (double)dados[i].ValorPago;
            }
        }, "Por Categoria");

        return File(bytes, ContentType, "PetCore_PorCategoria.xlsx");
    }

    // 13. GET /api/relatorios/por-tutor
    [HttpGet("por-tutor")]
    public async Task<IActionResult> PorTutor([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var query = _db.TransacoesFinanceiras
            .Where(t => t.ClinicaId == ClinicaId && t.TutorId != null)
            .Include(t => t.Tutor)
            .AsQueryable();

        if (dataInicio.HasValue) query = query.Where(t => t.DataVencimento >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(t => t.DataVencimento <= dataFim.Value);

        var dados = await query
            .GroupBy(t => new { t.TutorId, TutorNome = t.Tutor!.Nome })
            .Select(g => new
            {
                Tutor = g.Key.TutorNome,
                Quantidade = g.Count(),
                TotalReceitas = g.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor),
                TotalDespesas = g.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor),
                TotalPago = g.Sum(t => t.ValorPago ?? 0),
                TotalPendente = g.Where(t => t.Status == StatusTransacao.Pendente || t.Status == StatusTransacao.Atrasado).Sum(t => t.Valor - (t.ValorPago ?? 0))
            })
            .OrderByDescending(x => x.TotalReceitas)
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Tutor", "Qtd Transacoes", "Total Receitas", "Total Despesas", "Total Pago", "Total Pendente"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:F1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].Tutor;
                ws.Cell(r, 2).Value = dados[i].Quantidade;
                ws.Cell(r, 3).Value = (double)dados[i].TotalReceitas;
                ws.Cell(r, 4).Value = (double)dados[i].TotalDespesas;
                ws.Cell(r, 5).Value = (double)dados[i].TotalPago;
                ws.Cell(r, 6).Value = (double)dados[i].TotalPendente;
            }
        }, "Por Tutor");

        return File(bytes, ContentType, "PetCore_PorTutor.xlsx");
    }

    // 14. GET /api/relatorios/centros-custo
    [HttpGet("centros-custo")]
    public async Task<IActionResult> CentrosCusto()
    {
        var dados = await _db.CentrosCusto
            .Where(c => c.ClinicaId == ClinicaId)
            .Select(c => new
            {
                c.Nome,
                c.Descricao,
                c.Ativo,
                TotalTransacoes = c.Transacoes.Count,
                TotalReceitas = c.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor),
                TotalDespesas = c.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor),
                Saldo = c.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor)
                      - c.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor)
            })
            .OrderBy(c => c.Nome)
            .ToListAsync();

        var bytes = GerarExcel(ws =>
        {
            string[] headers = ["Nome", "Descricao", "Ativo", "Total Transacoes", "Total Receitas", "Total Despesas", "Saldo"];
            for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
            ws.Range("A1:G1").Style.Font.Bold = true;

            for (int i = 0; i < dados.Count; i++)
            {
                var r = i + 2;
                ws.Cell(r, 1).Value = dados[i].Nome;
                ws.Cell(r, 2).Value = dados[i].Descricao;
                ws.Cell(r, 3).Value = dados[i].Ativo ? "Sim" : "Nao";
                ws.Cell(r, 4).Value = dados[i].TotalTransacoes;
                ws.Cell(r, 5).Value = (double)dados[i].TotalReceitas;
                ws.Cell(r, 6).Value = (double)dados[i].TotalDespesas;
                ws.Cell(r, 7).Value = (double)dados[i].Saldo;
            }
        }, "Centros de Custo");

        return File(bytes, ContentType, "PetCore_CentrosCusto.xlsx");
    }
}
