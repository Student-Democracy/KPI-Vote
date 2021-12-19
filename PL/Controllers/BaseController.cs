using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PL.Controllers
{
    public abstract class BaseController : Controller
    {
        internal string UserId => !User.Identity.IsAuthenticated
            ? null
            : User.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}
