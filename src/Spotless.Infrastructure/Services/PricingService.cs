using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;

namespace Spotless.Infrastructure.Services
{
    public class PricingService : IPricingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PricingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<PriceCalculationResult>> GetItemPricesAsync(IReadOnlyList<OrderItemDto> items)
        {
            var results = new List<PriceCalculationResult>();


            var serviceIds = items.Select(i => i.ServiceId).Distinct().ToList();


            var services = await _unitOfWork.Services.GetByIdsAsync(serviceIds);



            foreach (var itemDto in items)
            {

                var service = services.FirstOrDefault(s => s.Id == itemDto.ServiceId);


                if (service == null)
                {
                    throw new KeyNotFoundException($"Service with ID {itemDto.ServiceId} not found.");
                }

                var itemPrice = service.PricePerUnit.Multiply(itemDto.Quantity);

                results.Add(new PriceCalculationResult(itemDto.ServiceId, itemPrice));
            }

            return results;
        }

        public Money CalculateTotal(IReadOnlyList<PriceCalculationResult> itemPrices)
        {

            if (itemPrices == null || !itemPrices.Any())
            {
                return Money.Zero;
            }

            var currency = itemPrices.First().Price.Currency;
            decimal totalAmount = itemPrices.Sum(p => p.Price.Amount);

            return new Money(totalAmount, currency);
        }
    }
}
