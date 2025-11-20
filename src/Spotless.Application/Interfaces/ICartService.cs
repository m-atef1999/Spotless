using Spotless.Application.Dtos.Cart;

namespace Spotless.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto?> GetCartAsync(Guid customerId);
        Task AddToCartAsync(Guid customerId, AddToCartDto dto);
        Task RemoveFromCartAsync(Guid customerId, Guid serviceId);
        Task ClearCartAsync(Guid customerId);
    }
}
