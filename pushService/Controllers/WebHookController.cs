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
        public IActionResult WebHook([FromBody] Payload payload) {
            // logger.LogDebug();
            StringValues eventName = Request.Headers["X-Coding-Event"];
            StringValues id = Request.Headers["X-Coding-Delivery"];
            StringValues signature = Request.Headers["X-Coding-Signature"];
            // Payload h = (Payload)payload;
            logger.LogDebug($"eventName:{eventName},id:{id}");
            logger.LogDebug(payload.ToString());
            logger.LogDebug(payload.GetType().Name);

            return Ok(payload);
        }
    }
}
