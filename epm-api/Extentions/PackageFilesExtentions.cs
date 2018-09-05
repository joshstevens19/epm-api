using System.Collections.Generic;
using epm_api.Models;

namespace epm_api.Extentions
{
    public static class PackageFilesExtentions
    {
        public static IReadOnlyCollection<S3File> ToS3Files(this IReadOnlyCollection<PackageFile> packageFiles)
        {
            IList<S3File> s3Files = new List<S3File>();
            foreach (var file in packageFiles)
            {
                s3Files.Add(new S3File(file.FileName, file.FileContent));
            }

            return (IReadOnlyCollection<S3File>)s3Files;
        }
    }
}
