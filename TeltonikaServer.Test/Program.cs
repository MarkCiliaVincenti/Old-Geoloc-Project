namespace TeltonikaServer.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var listner = DependencyRegistrar.ResolveDependencies();
            listner.Start();
        }
    }
}
