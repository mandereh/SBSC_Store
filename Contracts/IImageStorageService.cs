using System.IO;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IImageStorageService
    {
        // Uploads stream under the given fileName (path) and returns a publicly accessible URL.
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
        // Optional: delete a file by name/path
        Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
    }
}