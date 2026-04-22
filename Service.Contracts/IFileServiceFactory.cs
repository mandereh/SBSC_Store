using Entities.Enums;

namespace Service.Contracts;

public interface IFileServiceFactory
{
    IFileService Create(FileServiceType type);
}