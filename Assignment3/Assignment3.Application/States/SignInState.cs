using Assignment3.Application.Services;
using Assignment3.Domain.Models;

namespace Assignment3.Application.States;

internal class SignInState : AppState
{
    private readonly ConsoleService _consoleService;
    public SignInState(
        ConsoleService consoleService)
    {
        _consoleService = consoleService;
    }

    public override void Run()
    {
    }
}
