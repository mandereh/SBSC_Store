using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SBSC_Store.Configurations;
using Service.Contracts;

namespace Service;

public class CloudinaryFileService : IFileService
{
    private readonly Cloudinary _cloudClient;
    private readonly CloudinarySettings _cloudinaryConfig;
    private readonly ILoggerManager _logger;
    
    public CloudinaryFileService(IOptions<CloudinarySettings> config, ILoggerManager logger)
    {
        _logger = logger;
        _cloudinaryConfig = config.Value;
        var account = $"cloudinary://{_cloudinaryConfig.ApiKey}:{_cloudinaryConfig.ApiSecret}@{_cloudinaryConfig.CloudName}";
        _cloudClient = new Cloudinary(account);
    }
    
    public async Task<bool> DeleteFile(string fileName)
    {
        try
        {
            var deletionParams = new DeletionParams(fileName);
            var result = await _cloudClient.DestroyAsync(deletionParams);
            return result.Result == "ok";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting file {fileName}: {ex.Message}");
            return false;
        }
    }
    
    public async Task<string> UploadFile(IFormFile file, string fileName)
    {
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream)
        };

        var uploadResult = await _cloudClient.UploadAsync(uploadParams);
        return uploadResult.SecureUri.ToString();
    }
}