using Assignment3.Application.Services;

namespace Assignment3.Application.Controllers;
internal class AppController
{
    private readonly ConsoleService _consoleService;
    public AppController(ConsoleService consoleService)
    {
        _consoleService = consoleService;
    }

    public void Initialize()
    {
        Console.WriteLine("Welcome to All Your Healthy Food Store!");
        var input = _consoleService.AskForUserInput(new Dictionary<char, string>()
            {
                { 'S', "Sign In" },
                { 'B', "Browse Our Store" },
            });

        switch (input)
        {
            case 'S':
                Console.WriteLine("Sign In Page");
                break;
            case 'B':
                ShowProducts();
                break;
            default:
                throw new Exception();
        }
    }

    private void ShowProducts()
    {

    }
}
