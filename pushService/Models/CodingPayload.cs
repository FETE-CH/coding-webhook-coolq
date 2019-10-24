namespace pushService.Models {
    public class CodingPayload {
        //事件触发者
        public object Sender { set; get; }
        //当前项目
        public object Repository { set; get; }
    }
}
