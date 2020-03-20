using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PlagMShttp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlagController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return Ok(new PlagMS.Service().IsPlag(value));
        }
    }
}
