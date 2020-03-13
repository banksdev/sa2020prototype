using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileMShttp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        // GET: api/Files/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(Guid id)
        {
            return Ok(new FileMS.Service().GetFile(id));
        }

        // POST: api/Files
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            var fileId = new FileMS.Service().CreateFile(value);
            return Created($"{fileId}", fileId);
        }
    }
}
