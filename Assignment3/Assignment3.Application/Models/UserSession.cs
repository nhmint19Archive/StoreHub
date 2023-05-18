using Assignment3.Domain.Models;

namespace Assignment3.Application.Models;

internal class UserSession
{
    private UserAccount? _currentAccount;
    
    /// <summary>
    /// Gets whether a user is signed into the session.
    /// </summary>
    public bool IsUserSignedIn => _currentAccount != null;

    /// <summary>
    /// Gets the current user of the session.
    /// 
    /// </summary>
    /// <returns></returns>
    public UserAccount CurrentUser => _currentAccount ?? throw new InvalidOperationException("User is not signed in");

    /// <summary>
    /// Add the signed in account to the session.
    /// </summary>
    /// <param name="currentUser"></param>
    public void SignIn(UserAccount currentUser)
    {
        _currentAccount = currentUser;
    }

    /// <summary>
    /// Remove the signed in account from the session.
    /// </summary>
    public void SignOut()
    {
        _currentAccount = null;
    }
}
