using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Dtos
{
    public class PackageFilesRequestDto
    {
        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileContent { get; set; }
    }
}
