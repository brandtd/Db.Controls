namespace ParrotControl
{
    public class ConnectResult : IConnectResult
    {
        public Bebop Drone { get; set; }
        public string Error { get; set; }
    }
}