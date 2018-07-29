namespace ParrotControl
{
    public interface IConnectResult
    {
        Bebop Drone { get; }
        string Error { get; }
    }
}