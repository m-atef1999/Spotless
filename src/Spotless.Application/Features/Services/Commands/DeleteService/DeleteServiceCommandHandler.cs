using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Application.Services;

namespace Spotless.Application.Features.Services.Commands.DeleteService
{
    public class DeleteServiceCommandHandler(IUnitOfWork unitOfWork, CachedServiceService cachedServiceService) 
        : IRequestHandler<DeleteServiceCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly CachedServiceService _cachedServiceService = cachedServiceService;

        public async Task<Unit> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(request.ServiceId) 
                ?? throw new KeyNotFoundException($"Service with ID {request.ServiceId} not found.");

            await _unitOfWork.Services.DeleteAsync(service);
            await _unitOfWork.CommitAsync();
            
            // Invalidate cache
            await _cachedServiceService.InvalidateServiceCacheAsync();

            return Unit.Value;
        }
    }
}
