using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Models
{
    public class S3File
    {
        public S3File(string name, string content)
        {
            this.Name = name;
            this.Content = content;
        }

        public string Name { get; set; }

        public string Content { get; set; }
    }
}
