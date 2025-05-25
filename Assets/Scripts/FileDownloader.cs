using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

class FileDownloader
{

    public static async Task Main(string[] args)
    {
        string fileUrl = "https://example.com/sample.xlsx"; // Cambia por tu link
        string downloadFolder = "downloads"; // Carpeta donde guardarás el archivo

        try
        {
            // Crear el directorio si no existe
            Directory.CreateDirectory(downloadFolder);

            // Descargar el archivo y obtener el nombre
            string downloadedFileName = await DownloadFileWithFileNameAsync(fileUrl, downloadFolder);

            Console.WriteLine($"Archivo descargado: {downloadedFileName}");

            // Verificar si es un archivo Excel
            if (IsExcelFile(downloadedFileName))
            {
                Console.WriteLine("El archivo descargado es un archivo Excel.");
            }
            else
            {
                Console.WriteLine("El archivo descargado no es un archivo Excel.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    public static async Task<string> DownloadFileWithFileNameAsync(string url, string downloadFolder)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"No se pudo descargar el archivo. Código de estado: {response.StatusCode}");
            }

            // Obtener el nombre del archivo desde Content-Disposition
            string fileName = GetFileNameFromContentDisposition(response.Content.Headers);

            // Si no hay un nombre en los encabezados, intenta extraerlo de la URL
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Path.GetFileName(new Uri(url).LocalPath);
            }

            // Ruta completa del archivo descargado
            string filePath = Path.Combine(downloadFolder, fileName);

            // Descargar y guardar el archivo
            using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                          fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await contentStream.CopyToAsync(fileStream);
            }

            return filePath; // Devuelve la ruta completa del archivo descargado
        }
    }

    // Obtener el nombre del archivo desde el encabezado Content-Disposition
    static string GetFileNameFromContentDisposition(HttpContentHeaders headers)
    {
        if (headers.ContentDisposition != null)
        {
            return headers.ContentDisposition.FileName?.Trim('"'); // Elimina las comillas alrededor del nombre del archivo
        }
        return null;
    }

    // Método para verificar si el archivo es Excel
    static bool IsExcelFile(string filePath)
    {
        string[] validExcelMimeTypes = new string[]
        {
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // Excel moderno (xlsx)
            "application/vnd.ms-excel" // Excel antiguo (xls)
        };

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            byte[] buffer = new byte[512];
            fileStream.Read(buffer, 0, buffer.Length);

            string mimeType = GetMimeType(buffer);
            return Array.Exists(validExcelMimeTypes, mime => mime == mimeType);
        }
    }

    static string GetMimeType(byte[] fileHeader)
    {
        if (fileHeader.Length < 512)
            return string.Empty;

        if (fileHeader[0] == 0x50 && fileHeader[1] == 0x4B && fileHeader[2] == 0x03 && fileHeader[3] == 0x04)
        {
            return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
        else if (fileHeader[0] == 0xD0 && fileHeader[1] == 0xCF && fileHeader[2] == 0x11 && fileHeader[3] == 0xE0)
        {
            return "application/vnd.ms-excel";
        }

        return string.Empty;
    }

    public static async Task DownloadFileAsyncDrive(string fileId, string outputPath, string credentialsPath)
    {
        // 1. Autenticación: Carga credenciales
        GoogleCredential credential;
        using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(DriveService.Scope.DriveReadonly);
        }

        // 2. Crear el servicio de Drive
        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "MyDriveDownloader",
        });

        // 3. Obtener metadatos del archivo (opcional, para saber el nombre, mimeType, etc.)
        var fileMetadata = await service.Files.Get(fileId).ExecuteAsync();

        // 4. Preparar la solicitud de descarga
        var request = service.Files.Get(fileId);
        
        // 5. Descargar en un stream
        using (var memoryStream = new MemoryStream())
        {
            await request.DownloadAsync(memoryStream);

            // 6. Guardar en disco
            using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(fileStream);
            }
        }

        // Como "fileMetadata" tienes:
        // fileMetadata.Name: nombre del archivo en Drive
        // fileMetadata.MimeType: MIME type
        // fileMetadata.Size: tamaño
    }
}