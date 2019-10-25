using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace pushService.Services {
    public class QQMessageService {
        private HttpClient HttpClient { get; }

        private readonly IConfiguration config;
        private readonly ILogger<QQMessageService> logger;

        public QQMessageService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<QQMessageService> logger) {
            config = configuration;
            this.logger = logger;

            string coolQ = config["QQBotSetting:address"];
            string port = config["QQBotSetting:port"];
            HttpClient = httpClientFactory.CreateClient();
            HttpClient.BaseAddress = new Uri($"http://{coolQ}:{port}");
            logger.LogDebug($"http://{coolQ}:{port}");
            HttpClient.Timeout = new TimeSpan(TimeSpan.TicksPerSecond * 5);
        }

        private JsonElement CoolQMessageFormat(string resultJson) {
            JsonDocument jsonDocument = JsonDocument.Parse(resultJson);
            return jsonDocument.RootElement;
        }

        /// <summary>
        /// 获取 酷q 运行状态
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetCoolQStatus() {
            string result = await HttpClient.GetStringAsync("/get_status");
            JsonElement resultJson = CoolQMessageFormat(result);
            JsonElement data = resultJson.GetProperty("data");
            return data.GetProperty("good").GetBoolean();
        }
        
        
        public async Task<int> SendMessage(int qq,string msg) {
            bool coolQStatus = await GetCoolQStatus();
            if (!coolQStatus) {
                return -1;
            }

            // int groupNumber = int.Parse(config["QQBotSetting:targetQQGroup"]);
            // logger.LogDebug(groupNumber.ToString());
            var parameter = new {user_id = qq, message = msg};
            StringContent content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            
            HttpResponseMessage response = await HttpClient.PostAsync("/send_private_msg", content);

            string result = await response.Content.ReadAsStringAsync();
            JsonElement resultJson = CoolQMessageFormat(result);
            JsonElement data = resultJson.GetProperty("data");

            return data.GetProperty("message_id").GetInt32();
        }
        
        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<int> SendGroupMessage(string msg) {
            bool coolQStatus = await GetCoolQStatus();
            if (!coolQStatus) {
                return -1;
            }

            int groupNumber = int.Parse(config["QQBotSetting:targetQQGroup"]);
            logger.LogDebug(groupNumber.ToString());
            var parameter = new {group_id = groupNumber, message = msg};
            StringContent content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8);
            
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            HttpResponseMessage response = await HttpClient.PostAsync("/send_group_msg", content);

            string result = await response.Content.ReadAsStringAsync();
            JsonElement resultJson = CoolQMessageFormat(result);
            JsonElement data = resultJson.GetProperty("data");

            return data.GetProperty("message_id").GetInt32();
        }
    }
}
