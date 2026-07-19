namespace DentalClinic.Core.Common;

/// <summary>
/// Classe utilitária para padronizar retornos da camada de Application.
/// </summary>
public class Result<T>
{
    public bool Success { get; private set; }
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result(bool success, T? data, string? message)
    {
        Success = success;
        Data = data;
        Message = message;
    }

    public static Result<T> Ok(T data) => new(true, data, null);

    public static Result<T> Failure(string message) => new(false, default, message);

    public static Result<T> Failure(List<string> errors)
    {
        var result = new Result<T>(false, default, "Ocorreram um ou mais erros de validação.");
        result.Errors.AddRange(errors);
        return result;
    }
}
