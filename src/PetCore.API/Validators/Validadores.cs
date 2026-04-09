using FluentValidation;
using PetCore.API.Controllers;

namespace PetCore.API.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-mail obrigatório.").EmailAddress().WithMessage("E-mail inválido.");
        RuleFor(x => x.Senha).NotEmpty().WithMessage("Senha obrigatória.").MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres.");
    }
}

public class CriarTutorDtoValidator : AbstractValidator<CriarTutorDto>
{
    public CriarTutorDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome obrigatório.").MaximumLength(200).WithMessage("Nome muito longo.");
        RuleFor(x => x.Cpf).MaximumLength(14).WithMessage("CPF inválido.");
        RuleFor(x => x.Telefone).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("E-mail inválido.");
        RuleFor(x => x.Estado).MaximumLength(2);
        RuleFor(x => x.Cep).MaximumLength(10);
    }
}

public class CriarPacienteDtoValidator : AbstractValidator<CriarPacienteDto>
{
    public CriarPacienteDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome do animal obrigatório.").MaximumLength(200);
        RuleFor(x => x.TutorId).NotEmpty().WithMessage("Tutor obrigatório.");
        RuleFor(x => x.EspecieId).NotEmpty().WithMessage("Espécie obrigatória.");
        RuleFor(x => x.Peso).GreaterThan(0).When(x => x.Peso.HasValue).WithMessage("Peso deve ser maior que zero.");
    }
}

public class CriarAgendamentoDtoValidator : AbstractValidator<CriarAgendamentoDto>
{
    public CriarAgendamentoDtoValidator()
    {
        RuleFor(x => x.PacienteId).NotEmpty().WithMessage("Paciente obrigatório.");
        RuleFor(x => x.Tipo).NotEmpty().WithMessage("Tipo de agendamento obrigatório.");
        RuleFor(x => x.DataHoraAgendada).NotEmpty().WithMessage("Data/hora obrigatória.");
        RuleFor(x => x.DuracaoMinutos).InclusiveBetween(5, 480).WithMessage("Duração deve ser entre 5 e 480 minutos.");
    }
}

public class CriarProntuarioDtoValidator : AbstractValidator<CriarProntuarioDto>
{
    public CriarProntuarioDtoValidator()
    {
        RuleFor(x => x.PacienteId).NotEmpty().WithMessage("Paciente obrigatório.");
    }
}

public class CriarPrescricaoDtoValidator : AbstractValidator<CriarPrescricaoDto>
{
    public CriarPrescricaoDtoValidator()
    {
        RuleFor(x => x.NomeMedicamento).NotEmpty().WithMessage("Nome do medicamento obrigatório.").MaximumLength(200);
    }
}

public class CriarInternacaoDtoValidator : AbstractValidator<CriarInternacaoDto>
{
    public CriarInternacaoDtoValidator()
    {
        RuleFor(x => x.PacienteId).NotEmpty().WithMessage("Paciente obrigatório.");
    }
}

public class CriarProdutoDtoValidator : AbstractValidator<CriarProdutoDto>
{
    public CriarProdutoDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome do produto obrigatório.").MaximumLength(200);
        RuleFor(x => x.CategoriaId).NotEmpty().WithMessage("Categoria obrigatória.");
        RuleFor(x => x.UnidadeId).NotEmpty().WithMessage("Unidade obrigatória.");
        RuleFor(x => x.EstoqueMinimo).GreaterThanOrEqualTo(0).WithMessage("Estoque mínimo não pode ser negativo.");
    }
}

public class CriarMovimentacaoDtoValidator : AbstractValidator<CriarMovimentacaoDto>
{
    public CriarMovimentacaoDtoValidator()
    {
        RuleFor(x => x.ProdutoId).NotEmpty().WithMessage("Produto obrigatório.");
        RuleFor(x => x.Quantidade).GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.");
    }
}

public class CriarTransacaoDtoValidator : AbstractValidator<CriarTransacaoDto>
{
    public CriarTransacaoDtoValidator()
    {
        RuleFor(x => x.Tipo).NotEmpty().WithMessage("Tipo obrigatório.");
        RuleFor(x => x.CategoriaFinanceiraId).NotEmpty().WithMessage("Categoria financeira obrigatória.");
        RuleFor(x => x.Descricao).NotEmpty().WithMessage("Descrição obrigatória.").MaximumLength(500);
        RuleFor(x => x.Valor).GreaterThan(0).WithMessage("Valor deve ser maior que zero.");
        RuleFor(x => x.DataVencimento).NotEmpty().WithMessage("Data de vencimento obrigatória.");
    }
}

public class CriarUsuarioDtoValidator : AbstractValidator<CriarUsuarioDto>
{
    public CriarUsuarioDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome obrigatório.").MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-mail obrigatório.").EmailAddress().WithMessage("E-mail inválido.");
        RuleFor(x => x.Senha).NotEmpty().WithMessage("Senha obrigatória.").MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres.");
    }
}
