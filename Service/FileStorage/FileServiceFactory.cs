using Entities.Enums;
using Microsoft.Extensions.DependencyInjection;
using Service.Contracts;

namespace Service;

public class FileServiceFactory : IFileServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public FileServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IFileService Create(FileServiceType type)
    {
        return type switch
        {
            FileServiceType.Cloudinary => _serviceProvider.GetRequiredService<CloudinaryFileService>(),
            FileServiceType.Local => _serviceProvider.GetRequiredService<LocalFileService>(),
            _ => throw new ArgumentException("Invalid file service type")
        };
    }
}