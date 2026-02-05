namespace OIG.Assessment.Blazor.Services;

public class CurrentUserState
{
    public Guid? CurrentUserId { get; private set; }

    public event Action? Changed;

    public void SetUser(Guid? userId)
    {
        CurrentUserId = userId;
        Changed?.Invoke();
    }
}

