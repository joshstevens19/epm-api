using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Models
{
    public class PackageFile
    {
        public Uri FileUrl { get; set; }
        public string LocationInPackage { get; set; }

        public PackageFile() { }

        public PackageFile(Uri fileUrl, string locationInPackage)
        {
            this.FileUrl = fileUrl;
            this.LocationInPackage = locationInPackage;
        }
    }
}
