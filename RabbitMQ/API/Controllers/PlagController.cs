using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlagController : ControllerBase
    {
        private string queueName = "plag_queue";
        private string responseQueueName = "plag_queue_responses";

        // POST: api/Plag
        [HttpPost]
        public IActionResult Post([FromBody] string filetext)
        {
            var textbytes = Encoding.ASCII.GetBytes(filetext);
            var messageId = Guid.NewGuid();

            var factory = new ConnectionFactory() { HostName = "172.100.18.2" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                

                var messageWithId = messageId + ";" + "GET" + ";" + textbytes;
                RabbitMQHelper.SendMessage(channel, queueName, messageWithId);

            }

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: responseQueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                var messageWithId = messageId + ";" + "GET" + ";" + textbytes;
                RabbitMQHelper.SendMessage(channel, responseQueueName, messageWithId);

                // RECEIVE
                var response = RabbitMQHelper.GetMessageToId(channel, responseQueueName, messageId.ToString());

                return Ok(response);
        }

        }
    }
}
