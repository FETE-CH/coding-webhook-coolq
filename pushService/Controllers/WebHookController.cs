using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using pushService.Models;
using pushService.Services;

namespace pushService.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class WebHookController : ControllerBase {
        private readonly IConfiguration _config;
        private readonly ILogger<WebHookController> _logger;
        public readonly QQMessageService qqMessageService;

        public WebHookController(ILogger<WebHookController> logger, IConfiguration configuration, QQMessageService qqService) {
            _logger = logger;
            _config = configuration;
            qqMessageService = qqService;
        }

        // post /api/webhook
        [HttpPost]
        public async Task<IActionResult> WebHook([FromBody] object codingPayload) {
            StringValues eventName = Request.Headers["X-Coding-Event"];
            StringValues id = Request.Headers["X-Coding-Delivery"];
            StringValues signature = Request.Headers["X-Coding-Signature"];

            _logger.LogDebug($"eventName:{eventName},id:{id},signature:{signature}");

            JsonElement payload = (JsonElement) codingPayload;

            // 发送者
            JsonElement sender = payload.GetProperty("sender");

            // 操作的仓库
            JsonElement repository = payload.GetProperty("repository");
            string senderName = sender.GetProperty("name").GetString();
            string repoName = repository.GetProperty("name").GetString();

            if (eventName.Equals("ci")) {
                string actionName = payload.GetProperty("action").GetString();

                // string ciName = payload.GetProperty("ci").GetProperty("name").GetString();
                string branchName = payload.GetProperty("ci").GetProperty("branch_selector").GetString();
                _logger.LogDebug(actionName);

                if (actionName.Equals("complete_build")) {
                    //构建完成
                    string buildStatus = payload.GetProperty("ci").GetProperty("build").GetProperty("status").GetString();
                    bool buildResult = payload.GetProperty("ci").GetProperty("build").GetProperty("build_result").GetBoolean();
                    string env = _config[$"gitBranchMap:{branchName}"];
                    if (buildStatus.Equals("FAILED") || !buildResult) {
                        // 构建失败
                        int messageId = await qqMessageService.SendGroupMessage($"{repoName} 项目{env}环境构建失败\n请速去查看！");
                        _logger.LogInformation($"消息已发送: {messageId}");
                    } else {
                        // 构建成功
                        int messageId = await qqMessageService.SendGroupMessage($"恭喜！{repoName} 项目{env}环境构建成功！");
                        _logger.LogInformation($"消息已发送: {messageId}");
                    }
                } else if (actionName.Equals("cancel_build")) {
                    //取消构建
                }
            }

            // else if (eventName.Equals("merge request")) {
            //     logger.LogDebug("合并请求");
            // } 
            else if (eventName.Equals("task")) {
                _logger.LogDebug("任务操作");
            }

            // else if (eventName.Equals("document")) {
            //     logger.LogDebug("文件操作");
            // }
            else if (eventName.Equals("member")) {
                _logger.LogDebug("成员操作");
            }

            return Ok(200);
        }
    }
}
