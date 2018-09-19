using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Models
{
    public class EthereumPmMetaData
    {
        public string Description { get; set; }
        public string GitHub { get; set; }
        public bool Private { get; set; }
        public string Team { get; set; }
        public IList<string> Keywords { get; set; }
    }
}
