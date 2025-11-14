using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IReadOnlyList<Payment>> GetPaymentsByOrderIdAsync(Guid orderId);
    }
}
