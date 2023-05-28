using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Enums;

namespace Assignment3.Application.States
{
    /// <summary>
    /// Allows a staff member to update product inventory and view refund requests.
    /// </summary>
    internal class StaffProfileState : AppState
    {
        private readonly UserSession _session;
        private readonly IConsoleView _view;
        private readonly IConsoleInputHandler _inputHandler;
        public StaffProfileState(UserSession session, IConsoleView view, IConsoleInputHandler inputHandler)
        {
            _session = session;
            _view = view;
            _inputHandler = inputHandler;
        }
        
        /// <inheritdoc />
        public override void Run()
        {
            if (!_session.IsUserInRole(Roles.Staff))
            {
                _view.Error("Invalid access to staff page");
                _view.Info("Signing out");
                _session.SignOut();
                OnStateChanged(this, nameof(MainMenuState));
            }

            var input = _inputHandler.AskUserOption(
            new Dictionary<char, string>()
            {
                { 'D', "View Data Sales" },
                { 'V', "View My Profile" },
                { 'M', "Manage Inventory" },
                { 'E', "Exit to Main Menu" },
            });

            switch (input)
            {
                case 'D':
                    OnStateChanged(this, nameof(ViewSalesDataState));
                    break;
                case 'M':
                    OnStateChanged(this, nameof(ManageInventoryState));
                    break;
                case 'E':
                    OnStateChanged(this, nameof(MainMenuState));
                    break;
                case 'V':
                    ShowProfile();
                    break;
            }
        }

        private void ShowProfile()
        {
            _view.Info($"Email: {_session.AuthenticatedUser.Email}");
            _view.Info($"Phone: {_session.AuthenticatedUser.Phone}");
            _view.Info($"Registration Date: {_session.AuthenticatedUser.RegistryDate.ToLocalTime()}");
        }
    }
}
