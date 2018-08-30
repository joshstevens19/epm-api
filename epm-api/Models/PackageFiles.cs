using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Models
{
    public class PackageFiles
    {
        public PackageFiles(string version, string packageName, IReadOnlyCollection<PackageFile> files)
        {
            this.Version = version;
            this.PackageName = packageName;
            this.Files = files;
        }

        public string PackageName { get; set; }

        public string Version { get; set; }

        public IReadOnlyCollection<PackageFile> Files { get; }
    }
}
