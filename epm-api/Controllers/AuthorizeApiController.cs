using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace epm_api.Controllers
{
    [Authorize]
    public class AuthorizeApiController : Controller
    {

    }
}
