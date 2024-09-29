
namespace FileUploadApp.Services
{
    public interface IUploadFileService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}