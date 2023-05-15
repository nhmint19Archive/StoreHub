using Microsoft.Extensions.DependencyInjection;

namespace Assignment3;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var serviceProvider = RegisterDependencies();
    }

    static ServiceProvider RegisterDependencies()
    {
        var services = new ServiceCollection();
        // TODO: register objects here
        return services.BuildServiceProvider();
    }
}
