using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

public static class FileDownloader
{
    private static readonly HttpClient _httpClient = new HttpClient();
    
    static FileDownloader()
    {
        // Configurar HttpClient una sola vez (mejor performance)
        _httpClient.Timeout = TimeSpan.FromMinutes(5);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
    }

    public static async Task<string> DownloadFileWithFileNameAsync(string url, string downloadFolder, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("La URL no puede estar vacía", nameof(url));

        if (string.IsNullOrEmpty(downloadFolder))
            throw new ArgumentException("La carpeta de destino no puede estar vacía", nameof(downloadFolder));

        // Validar que la URL sea válida
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) || 
            !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            throw new ArgumentException("La URL no es válida", nameof(url));
        }

        HttpResponseMessage response = await _httpClient.GetAsync(url, 
            HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        response.EnsureSuccessStatusCode();

        // Obtener el nombre del archivo
        string fileName = GetFileNameFromContentDisposition(response.Content.Headers) ?? 
                         Path.GetFileName(uriResult.LocalPath);

        // Si no se pudo determinar el nombre del archivo
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = $"downloaded_file_{DateTime.Now:yyyyMMddHHmmss}";
        }

        // Sanitizar nombre de archivo
        fileName = SanitizeFileName(fileName);

        // Crear directorio si no existe
        Directory.CreateDirectory(downloadFolder);

        // Ruta completa del archivo
        string filePath = Path.Combine(downloadFolder, fileName);

        // Descargar y guardar el archivo
        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await contentStream.CopyToAsync(fileStream, cancellationToken);
        }

        return filePath;
    }

    private static string GetFileNameFromContentDisposition(HttpContentHeaders headers)
    {
        if (headers.ContentDisposition != null)
        {
            return headers.ContentDisposition.FileName?.Trim('"');
        }
        return null;
    }

    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return "downloaded_file";

        // Eliminar caracteres inválidos para nombres de archivo
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c.ToString(), "_");
        }

        // Limitar longitud del nombre
        if (fileName.Length > 100)
        {
            string extension = Path.GetExtension(fileName);
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            nameWithoutExtension = nameWithoutExtension.Substring(0, Math.Min(100 - extension.Length, nameWithoutExtension.Length));
            fileName = nameWithoutExtension + extension;
        }

        return fileName;
    }

    public static bool IsExcelFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return false;

        try
        {
            string[] validExcelExtensions = new string[] { ".xlsx", ".xls" };
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            
            // Primera validación rápida por extensión
            if (Array.IndexOf(validExcelExtensions, extension) == -1)
                return false;

            // Validación más robusta leyendo la firma del archivo
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[8]; // Solo necesitamos los primeros bytes
                int bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                // Firma de archivos Excel (xlsx son ZIP, xls tiene firma específica)
                if (bytesRead >= 4)
                {
                    // XLSX: PK header (ZIP file)
                    if (buffer[0] == 0x50 && buffer[1] == 0x4B && buffer[2] == 0x03 && buffer[3] == 0x04)
                        return true;

                    // XLS: D0 CF 11 E0
                    if (buffer[0] == 0xD0 && buffer[1] == 0xCF && buffer[2] == 0x11 && buffer[3] == 0xE0)
                        return true;
                }
            }

            return false;
        }
        catch
        {
            // Si hay error al leer el archivo, asumimos que no es Excel válido
            return false;
        }
    }
}