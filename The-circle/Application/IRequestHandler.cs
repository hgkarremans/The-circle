using The_circle.Domain.Models;

namespace The_circle.Application;

public interface IRequestHandler<TRequest> where TRequest : IRequest
{
    Task<Unit> Handle(TRequest request, CancellationToken cancellationToken);
}
