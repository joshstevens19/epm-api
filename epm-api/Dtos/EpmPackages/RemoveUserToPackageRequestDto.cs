﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace epm_api.Packages.Dtos.EpmPackages
{
    public class RemoveUserToPackageRequestDto
    {
        [Required]
        public string PackageName { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
