using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using pushService.Models;
using pushService.Services;

namespace pushService.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class WebHookController : ControllerBase {
        private readonly ILogger<WebHookController> logger;
        private readonly QQMessageService qqMessageService;


        public WebHookController(ILogger<WebHookController> logger, QQMessageService qqService) {
            this.logger = logger;
            qqMessageService = qqService;
        }

        // post /api/webhook
        [HttpPost]
        public async Task<OkObjectResult> WebHook([FromBody] object codingPayload) {
            StringValues eventName = Request.Headers["X-Coding-Event"];
            StringValues id = Request.Headers["X-Coding-Delivery"];
            StringValues signature = Request.Headers["X-Coding-Signature"];

            // qqMessageService.Send();

            logger.LogDebug($"eventName:{eventName},id:{id},signature:{signature}");

            JsonElement payload = (JsonElement) codingPayload;

            // 发送者
            JsonElement sender = payload.GetProperty("sender");

            // 操作的仓库
            JsonElement repository = payload.GetProperty("repository");
            var senderName = sender.GetProperty("name").GetString();
            var repoName = repository.GetProperty("name").GetString();

            if (eventName.Equals("ci")) {
                string actionName = payload.GetProperty("action").GetString();
                // Task<int> sendMessage = qqMessageService.SendMessage(1979772544,$"通知\n构建啦😝");
                if (actionName == "trigger_build") {
                    // 触发构建
                    int messageId = await qqMessageService.SendGroupMessage($"通知\n构建啦啦啦😝");
                    logger.LogDebug(messageId.ToString());
                } else if (actionName == "complete_build") {
                    //构建成功
                } else if (actionName == "cancel_build") {
                    //取消构建
                }
            } else if (eventName.Equals("merge request")) {
                new object() { };
            } else if (eventName.Equals("task")) {
                new object() { };
            } else if (eventName.Equals("document")) {
                new object() { };
            } else if (eventName.Equals("member")) {
                new object() { };
            }

            return Ok(1);
        }
    }
}
