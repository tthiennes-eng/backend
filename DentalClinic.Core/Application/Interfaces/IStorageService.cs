namespace DentalClinic.Core.Application.Interfaces;

/// <summary>
/// Interface para armazenamento de arquivos (Radiografias, Fotos, Documentos).
/// Segue os requisitos de não armazenar binários no banco de dados.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Faz o upload de um arquivo e retorna a URL pública ou caminho interno.
    /// </summary>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);

    /// <summary>
    /// Remove um arquivo do armazenamento.
    /// </summary>
    Task DeleteFileAsync(string fileUrl);
}
