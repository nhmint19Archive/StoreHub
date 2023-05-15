using Microsoft.Extensions.DependencyInjection;

namespace Assignment3.Application;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        _ = RegisterDependencies();
    }

    private static ServiceProvider RegisterDependencies()
    {
        var services = new ServiceCollection();
        // TODO: register objects here
        return services.BuildServiceProvider();
    }
}
