using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    public class BaseApiController : Controller
    {
        public async Task<string> GetJwtToken()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }
    }
}
