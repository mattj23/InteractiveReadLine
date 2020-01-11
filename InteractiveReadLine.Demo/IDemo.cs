namespace InteractiveReadLine.Demo
{
    public interface IDemo
    {
        string Path { get; }
        string Description { get; }

        void Action();
    }
}