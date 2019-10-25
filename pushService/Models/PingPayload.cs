namespace pushService.Models {
    public class PingPayload : CodingPayload {
        public string Zen { set; get; }
        public int HookId { set; get; }
        public object Hook { set; get; }
    }
}
