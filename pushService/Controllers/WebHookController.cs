using System;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using pushService.Models;

namespace pushService.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class WebHookController : ControllerBase {
        private readonly ILogger<WebHookController> logger;

        public WebHookController(ILogger<WebHookController> logger) {
            this.logger = logger;
        }

        // post /api/webhook
        [HttpPost]
        public IActionResult WebHook([FromBody] PingPayload codingPayload) {
            // logger.LogDebug();
            StringValues eventName = Request.Headers["X-Coding-Event"];
            StringValues id = Request.Headers["X-Coding-Delivery"];
            StringValues signature = Request.Headers["X-Coding-Signature"];

            logger.LogDebug($"eventName:{eventName},id:{id},signature:{signature}");

            // logger.LogDebug(payload.GetType().Name);
            string json = JsonSerializer.Serialize(codingPayload);
            logger.LogDebug(json);


            return Ok(1);
        }
    }
}
