using Spotless.Application.Dtos.Cart;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;

namespace Spotless.Application.Services
{
    public class CartService(ICartRepository cartRepository, IServiceRepository serviceRepository) : ICartService
    {
        private readonly ICartRepository _cartRepository = cartRepository;
        private readonly IServiceRepository _serviceRepository = serviceRepository;

        public async Task<CartDto?> GetCartAsync(Guid customerId)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null) return null;

            var services = await _serviceRepository.GetByIdsAsync(cart.Items.Select(i => i.ServiceId));
            var weightMap = services.ToDictionary(s => s.Id, s => s.MaxWeightKg);

            var items = cart.Items.Select(i => new CartItemDto(i.Id, i.ServiceId, services.First(s => s.Id == i.ServiceId).Name, i.Quantity, weightMap[i.ServiceId], i.AddedDate)).ToList();

            var totalWeight = cart.CalculateTotalWeightKg(weightMap);

            return new CartDto(cart.Id, cart.CustomerId, items, totalWeight, cart.CreatedDate, cart.LastModifiedDate);
        }

        public async Task AddToCartAsync(Guid customerId, AddToCartDto dto)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId) ?? new Cart(customerId);

            // Validate service existence
            var service = await _serviceRepository.GetByIdAsync(dto.ServiceId) ?? throw new InvalidOperationException("Service does not exist.");
            cart.AddOrUpdateItem(dto.ServiceId, dto.Quantity);

            await _cartRepository.AddOrUpdateCartAsync(cart);
        }

        public async Task RemoveFromCartAsync(Guid customerId, Guid serviceId)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null) return;

            cart.RemoveItem(serviceId);
            await _cartRepository.AddOrUpdateCartAsync(cart);
        }

        public async Task ClearCartAsync(Guid customerId)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null) return;

            cart.Clear();
            await _cartRepository.AddOrUpdateCartAsync(cart);
        }
    }
}
