using MotoStore.Api.Models;

public class AuthContext
{
    public User? CurrentUser { get; private set; }

    public void SignIn(User user)
    {
        CurrentUser = user;
    }

    public void SignOut()
    {
        CurrentUser = null;
    }

    public bool IsAuthenticated => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "admin";
}