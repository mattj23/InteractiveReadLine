namespace InteractiveReadLine
{
    public class HandlerConfig
    {
        public bool IsTesting { get; set; }

        public static HandlerConfig Test() => new HandlerConfig() {IsTesting = true};
    }
}