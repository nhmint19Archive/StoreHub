using Assignment3.Domain.Models;

namespace Assignment3.Application.Models;

internal class UserSession
{
    private UserAccount? _currentAccount;
    public bool IsUserSignedIn => _currentAccount != null;
    public UserAccount CurrentUser => _currentAccount ?? throw new InvalidOperationException("User is not signed in");
    public void SignIn(UserAccount currentUser)
    {
        _currentAccount = currentUser;
    }
    public void SignOut()
    {
        _currentAccount = null;
    }
}
