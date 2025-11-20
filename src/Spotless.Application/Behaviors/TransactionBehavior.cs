using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {

            if (request is IQuery)
            {
                return await next();
            }

            try
            {


                var response = await next();


                await _unitOfWork.CommitAsync();

                return response;
            }
            catch
            {

                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}