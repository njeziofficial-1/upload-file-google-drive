using FileUploadApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController(IUploadFileService fileService) : ControllerBase
{
    private readonly IUploadFileService _fileService = fileService;

    [HttpPost("upload-file")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var fileUri = await _fileService.UploadAsync(file);
        return Ok(fileUri);
    }
}
