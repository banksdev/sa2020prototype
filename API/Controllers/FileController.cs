using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {

        // GET: api/File/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(Guid id)
        {
            var request = WebRequest.Create($"https://file_service/api/files/{id}");
            request.Credentials = CredentialCache.DefaultCredentials;
            var response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            string responseFromServer = null;
            using (Stream data = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(data);
                responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
            }

            response.Close();
            return Ok(responseFromServer);
        }

        // POST: api/File
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}
