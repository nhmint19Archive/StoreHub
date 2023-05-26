using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Application.States
{
    internal class StaffProfileState : AppState
    {
        private readonly UserSession _session;
        public StaffProfileState(UserSession session)
        {
            _session = session;
        }
        public override void Run()
        {
            if (!_session.IsUserInRole(Roles.Staff))
            {
                ConsoleHelper.PrintError("Invalid access to customer page");
                ConsoleHelper.PrintInfo("Signing out");
                _session.SignOut();
                OnStateChanged(this, nameof(MainMenuState));
            }

            ConsoleHelper.PrintInfo("Staff Profile");
            ConsoleHelper.PrintInfo($"Email: {_session.AuthenticatedUser.Email}");
            ConsoleHelper.PrintInfo($"Phone: {_session.AuthenticatedUser.Phone}");
            ConsoleHelper.PrintInfo($"Registration Date: {_session.AuthenticatedUser.RegistryDate.ToLocalTime()}");

            var input = ConsoleHelper.AskUserOption(
            new Dictionary<char, string>()
            {
                { 'V', "View Data Sales"},
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
