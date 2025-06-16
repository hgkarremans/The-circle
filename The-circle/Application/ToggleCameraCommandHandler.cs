using The_circle.Domain;
using The_circle.Domain.Models;

namespace The_circle.Application;

public record ToggleCameraCommand(Guid UserId, bool TurnOn) : IRequest;

public class ToggleCameraCommandHandler : IRequestHandler<ToggleCameraCommand>
{
    private readonly IUserCameraReadRepository _readRepository;
    private readonly IUserCameraWriteRepository _writeRepository;

    public ToggleCameraCommandHandler(IUserCameraReadRepository readRepository, IUserCameraWriteRepository writeRepository)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
    }


    public async Task<Unit> Handle(ToggleCameraCommand request, CancellationToken cancellationToken)
    {
        var userCamera = await _readRepository.GetByUserIdAsync(request.UserId);
        if (userCamera == null)
        {
            userCamera = new UserCamera(request.UserId);
            await _writeRepository.AddAsync(userCamera);
        }

        if (request.TurnOn)
            userCamera.TurnOn();
        else
            userCamera.TurnOff();

        await _writeRepository.SaveAsync(userCamera);

        return Unit.Value;
    }
}
