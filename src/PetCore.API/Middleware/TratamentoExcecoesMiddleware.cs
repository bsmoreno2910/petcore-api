using System.Net;
using System.Text.Json;

namespace PetCore.API.Middleware;

public class TratamentoExcecoesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TratamentoExcecoesMiddleware> _logger;

    public TratamentoExcecoesMiddleware(RequestDelegate next, ILogger<TratamentoExcecoesMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Mensagem}", ex.Message);
            await TratarExcecaoAsync(context, ex);
        }
    }

    private static async Task TratarExcecaoAsync(HttpContext context, Exception exception)
    {
        var (statusCode, mensagem) = exception switch
        {
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Acesso não autorizado."),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Recurso não encontrado."),
            _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno no servidor.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var resposta = JsonSerializer.Serialize(new { erro = mensagem });
        await context.Response.WriteAsync(resposta);
    }
}

public static class TratamentoExcecoesMiddlewareExtensions
{
    public static IApplicationBuilder UseTratamentoExcecoes(this IApplicationBuilder builder) =>
        builder.UseMiddleware<TratamentoExcecoesMiddleware>();
}
