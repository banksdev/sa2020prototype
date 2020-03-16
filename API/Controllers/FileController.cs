using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
        public IActionResult Post([FromBody] string filetext)
        {
            var textbytes = Encoding.ASCII.GetBytes(filetext);
            var request = WebRequest.Create($"https://file_service/api/files");
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentLength = textbytes.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(textbytes, 0, textbytes.Length);
            requestStream.Close();

            var response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            string responseFromServer = null;
            using (var responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream);
                responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
            }

            response.Close();

            return Ok(responseFromServer);
        }
    }
}
