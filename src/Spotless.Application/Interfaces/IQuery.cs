using MediatR;

namespace Spotless.Application.Interfaces
{

    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }


    public interface IQuery
    {
    }
}
