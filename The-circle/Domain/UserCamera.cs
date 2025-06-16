using The_circle.Domain.Models;

namespace The_circle.Domain;

public class UserCamera : IUserCamera
{
    public Guid UserId { get; private set; }
    public bool IsOn { get; private set; }

    public UserCamera(Guid userId)
    {
        UserId = userId;
        IsOn = false;
    }

    public void TurnOn() => IsOn = true;
    public void TurnOff() => IsOn = false;
}