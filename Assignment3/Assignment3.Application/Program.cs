using Assignment3.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment3;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var services = new ServiceCollection();
        services.AddScoped<IInputController, InputController>();
        var serviceProvider = services.BuildServiceProvider();

        // Obtain an instance of the InputController object
        var inputController = serviceProvider.GetService<IInputController>();
    }
}
