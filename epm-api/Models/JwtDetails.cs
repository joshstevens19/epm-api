using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epm_api.Models
{
    public class JwtDetails
    {
        public JwtDetails(string token, DateTime expiryDate)
        {
            this.Token = token;
            this.ExpiryDate = expiryDate;
        }

        public string Token { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}
