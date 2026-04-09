namespace PetCore.Domain.Interfaces;

public interface IServicoAutenticacao
{
    Task<ResultadoAutenticacao> LoginAsync(string email, string senha);
    Task<ResultadoAutenticacao> RefreshTokenAsync(string tokenAtualizacao);
    Task AlterarSenhaAsync(Guid usuarioId, string senhaAtual, string novaSenha);
    Task<ResultadoAutenticacao> SelecionarClinicaAsync(Guid usuarioId, Guid clinicaId);
}

public class ResultadoAutenticacao
{
    public bool Sucesso { get; set; }
    public string? TokenAcesso { get; set; }
    public string? TokenAtualizacao { get; set; }
    public object? Usuario { get; set; }
    public object? Clinicas { get; set; }
    public string? Erro { get; set; }
}
