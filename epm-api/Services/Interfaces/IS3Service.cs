using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3;
using epm_api.Models;

namespace epm_api.Services.Interfaces
{
    public interface IS3Service
    {
        IAmazonS3 GetClient();
        string GetBucketName();
        Task UploadFileAsync(string fileContent, string keyName);
        Task UploadFilesAsync(IReadOnlyCollection<S3File> filesContent, string rootkeyName);
    }
}