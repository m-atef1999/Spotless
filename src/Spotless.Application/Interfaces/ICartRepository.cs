using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetCartByCustomerIdAsync(Guid customerId);
        Task AddOrUpdateCartAsync(Cart cart);
        Task RemoveCartAsync(Guid cartId);
    }
}
