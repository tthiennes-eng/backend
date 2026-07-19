using DentalClinic.Core.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DentalClinic.Infrastructure.Storage;

/// <summary>
/// Implementação de armazenamento local para desenvolvimento e produção inicial.
/// Pode ser facilmente substituída por S3 ou Azure Blob Storage implementando IStorageService.
/// </summary>
public class LocalStorageService : IStorageService
{
    private readonly string _storagePath;
    private readonly string _baseUrl;

    public LocalStorageService(IConfiguration configuration)
    {
        _storagePath = configuration["Storage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        _baseUrl = configuration["Storage:BaseUrl"] ?? "https://localhost:5001/uploads";

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var fileExtension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(_storagePath, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        return $"{_baseUrl}/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_storagePath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
