using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Models
{
    public class PackageFile
    {
        public PackageFile(string fileName, string fileContent)
        {
            FileName = fileName;
            FileContent = fileContent;
        }

        public string FileName { get; set; }

        public string FileContent { get; set; }
    }
}
