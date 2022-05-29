using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_ASP.NET_CORE_WEBAPI.Controllers
{
    [EnableCors("CORSAPI")]
    [Route("api")]
    [ApiController]
    public class BaseController : Controller
    {
    }
}
