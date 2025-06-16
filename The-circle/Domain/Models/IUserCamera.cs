namespace The_circle.Domain.Models;

public interface IUserCamera
{
    Guid UserId { get; }
    bool IsOn { get; }
        
    void TurnOn();
    void TurnOff();
}