using StoreHub.Domain.Enums;
using StoreHub.Domain.Models;

namespace StoreHub.Application.Models;

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
    public UserAccount AuthenticatedUser => _currentAccount ?? throw new InvalidOperationException("User is not signed in");

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

    /// <summary>
    /// Checks if the user is signed in and has a valid role.
    /// </summary>
    /// <param name="role">Allowed role.</param>
    /// <returns><c>True<c/>if the user is signed in with the correct role, otherwise <c>False<c/>.</returns>
    public bool IsUserInRole(Roles role)
    {
        return _currentAccount?.Role == role;
    }
}
