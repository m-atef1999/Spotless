using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class CartRepository(ApplicationDbContext dbContext) : BaseRepository<Cart>(dbContext), ICartRepository
    {
        public async Task<Cart?> GetCartByCustomerIdAsync(Guid customerId)
        {
            return await _dbContext.Set<Cart>()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task AddOrUpdateCartAsync(Cart cart)
        {
            var existing = await GetCartByCustomerIdAsync(cart.CustomerId);
            if (existing == null)
            {
                // New cart, just add
                await _dbContext.Set<Cart>().AddAsync(cart);
                return;
            }

            // Merge items: for each incoming item, update or add; remove any existing items not present in incoming
            var incomingByService = cart.Items.ToDictionary(i => i.ServiceId, i => i);

            // Update existing items or add new ones
            foreach (var incoming in incomingByService.Values)
            {
                var existItem = existing.Items.FirstOrDefault(i => i.ServiceId == incoming.ServiceId);
                if (existItem != null)
                {
                    // update quantity if different
                    if (existItem.Quantity != incoming.Quantity)
                    {
                        existItem.UpdateQuantity(incoming.Quantity);
                    }
                }
                else
                {
                    // add new item to the tracked existing cart (this will create a CartItem with existing.Id)
                    existing.AddOrUpdateItem(incoming.ServiceId, incoming.Quantity);
                }
            }

            // Remove items that exist in DB but not present in incoming cart
            var incomingServiceIds = incomingByService.Keys.ToHashSet();
            var itemsToRemove = existing.Items.Where(i => !incomingServiceIds.Contains(i.ServiceId)).ToList();
            foreach (var toRemove in itemsToRemove)
            {
                // detach from existing collection so EF will delete it (collection is tracked)
                _dbContext.Set<CartItem>().Remove(toRemove);
            }

            // mark existing cart as modified (EF is tracking existing and its children)
            _dbContext.Entry(existing).State = EntityState.Modified;
        }

        public async Task RemoveCartAsync(Guid cartId)
        {
            var cart = await _dbContext.Set<Cart>().FindAsync(cartId);
            if (cart != null)
                _dbContext.Set<Cart>().Remove(cart);
        }
    }
}
