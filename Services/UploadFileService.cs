using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace FileUploadApp.Services;

public class UploadFileService : IUploadFileService
{
    IConfiguration _configuration;

    public UploadFileService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        string? pathToKeyFile = _configuration["ServiceAccountCredentials:PathToKeyFile"];
        string? directoryId = _configuration["ServiceAccountCredentials:DirectoryId"];

        try
        {
            // Load the Service Account credentials and define the scope of its access.
            var credentials = GoogleCredential.FromFile(pathToKeyFile)
                .CreateScoped(DriveService.ScopeConstants.DriveFile);

            // Create the Drive service
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials
            });

            // Define file metadata including the target folder
            var fileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = file.FileName,
                Parents = [directoryId]
            };

            string? uploadedFileId;

            // Read the file from the IFormFile and upload it to Google Drive
            using var stream = file.OpenReadStream();
            var request = service.Files.Create(fileMetaData, stream, file.ContentType);
            request.Fields = "id";  // Only request the file ID in the response
            var results = await request.UploadAsync(CancellationToken.None);

            // Handle any errors during upload
            if (results.Status == Google.Apis.Upload.UploadStatus.Failed)
            {
                return $"Error uploading file: {results.Exception.Message}";
            }

            // Retrieve the file ID after successful upload
            uploadedFileId = request.ResponseBody?.Id;


            return uploadedFileId;
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            return $"Error: {ex.Message}";
        }
    }

    private static UserCredential GetCredentials()
    {
        throw new NotImplementedException();
    }

    static string UploadBasicImage(string path, DriveService service)
    {
        var fileMetaData = new Google.Apis.Drive.v3.Data.File();
        fileMetaData.Name = Path.GetFileName(path);
        fileMetaData.MimeType = "image/jpg";
        FilesResource.CreateMediaUpload request;
        using var stream = new FileStream(path, FileMode.Open);
        request = service.Files.Create(fileMetaData, stream, fileMetaData.MimeType);
        request.Fields = "id";
        request.Upload();
        var file = request.ResponseBody;
        return file.Id;
    }
}