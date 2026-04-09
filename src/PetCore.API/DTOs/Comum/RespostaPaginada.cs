namespace PetCore.API.DTOs.Comum;

public class RespostaPaginada<T>
{
    public List<T> Itens { get; set; } = [];
    public int TotalRegistros { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanhoPagina);
    public bool TemPaginaAnterior => Pagina > 1;
    public bool TemProximaPagina => Pagina < TotalPaginas;
}
