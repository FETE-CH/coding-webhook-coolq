namespace pushService.Models {
    public class PingPayload : CodingPayload {
        public string Zen { set; get; }
        public string HookId { set; get; }
        public object Hook { set; get; }
    }
}
