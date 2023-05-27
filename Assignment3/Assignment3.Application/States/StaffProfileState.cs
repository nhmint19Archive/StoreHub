using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Enums;

namespace Assignment3.Application.States
{
    internal class StaffProfileState : AppState
    {
        private readonly UserSession _session;
        private readonly IConsoleView _view;
        private readonly IConsoleInputHandler _inputHandler;
        public StaffProfileState(
            UserSession session, 
            IConsoleView view, 
            IConsoleInputHandler inputHandler)
        {
            _session = session;
            _view = view;
            _inputHandler = inputHandler;
        }
        public override void Run()
        {
            if (!_session.IsUserInRole(Roles.Staff))
            {
                _view.Error("Invalid access to customer page");
                _view.Info("Signing out");
                _session.SignOut();
                OnStateChanged(this, nameof(MainMenuState));
            }

            var input = _inputHandler.AskUserOption(
            new Dictionary<char, string>()
            {
                { 'V', "View Sales Data"},
                { 'M', "Manage Inventory"},
                { 'E', "Exit to Main Menu" },
            });

            switch (input)
            {
                case 'V':
                    OnStateChanged(this, nameof(ViewSalesDataState));
                    break;
                case 'M':
                    OnStateChanged(this, nameof(ManageInventoryState));
                    break;
                case 'E':
                    OnStateChanged(this, nameof(MainMenuState));
                    break;
            }
        }
    }
}
