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

            logger.LogDebug($"eventName:{eventName},id:{id},signature:{signature}");

            JsonElement payload = (JsonElement) codingPayload;

            // 发送者
            JsonElement sender = payload.GetProperty("sender");

            // 操作的仓库
            JsonElement repository = payload.GetProperty("repository");
            string senderName = sender.GetProperty("name").GetString();
            string repoName = repository.GetProperty("name").GetString();

            if (eventName.Equals("ci")) {
                string actionName = payload.GetProperty("action").GetString();
                logger.LogDebug(actionName);
                if (actionName.Equals("complete_build")) {
                    //构建成功
                    int messageId = await qqMessageService.SendMessage(1979772544, $"通知\n构建啦😝");
                    logger.LogInformation($"消息已发送: {messageId}");
                } else if (actionName.Equals("cancel_build")) {
                    //取消构建
                } else if (actionName.Equals("failed_build")) {
                    // 构建失败
                }
            } else if (eventName.Equals("merge request")) {
                logger.LogDebug($"合并请求");
            } else if (eventName.Equals("task")) {
                logger.LogDebug($"任务操作");
            } else if (eventName.Equals("document")) {
                logger.LogDebug($"文件操作");
            } else if (eventName.Equals("member")) {
                logger.LogDebug($"成员操作");
            }

            return Ok(payload);
        }
    }
}
