using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;

namespace Spotless.Infrastructure.Services
{
    public class PricingService(IUnitOfWork unitOfWork) : IPricingService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<PriceCalculationResult>> GetItemPricesAsync(IReadOnlyList<PricingItemDto> items)
        {
            var results = new List<PriceCalculationResult>();

            var serviceIds = items.Select(i => i.ServiceId).Distinct().ToList();

            var services = await _unitOfWork.Services.GetByIdsAsync(serviceIds);

            foreach (var itemDto in items)
            {
                var service = services.FirstOrDefault(s => s.Id == itemDto.ServiceId) ?? throw new KeyNotFoundException($"Service with ID {itemDto.ServiceId} not found.");
                var itemPrice = service.PricePerUnit.Multiply(itemDto.Quantity);

                results.Add(new PriceCalculationResult(itemDto.ServiceId, itemPrice, service.PricePerUnit));
            }

            return results;
        }

        public PriceEstimateDto CalculateTotal(IReadOnlyList<PriceCalculationResult> itemPrices)
        {
            if (itemPrices == null || !itemPrices.Any())
            {
                return new PriceEstimateDto(Money.Zero);
            }

            var currency = itemPrices.First().Price.Currency;
            decimal totalAmount = itemPrices.Sum(p => p.Price.Amount);

            var totalMoney = new Money(totalAmount, currency);

            return new PriceEstimateDto(totalMoney);
        }

        public async Task<PriceEstimateResponse> CalculatePriceEstimateAsync(CreateOrderDto orderDto, Guid customerId, CancellationToken cancellationToken)
        {
            // Extract pricing items from the order dto
            var pricingItems = orderDto.Items?.Select(item => 
                new PricingItemDto(
                    ServiceId: item.ServiceId,
                    ItemName: $"Service {item.ServiceId}", // Use ServiceId as fallback name
                    Quantity: item.Quantity
                )
            ).ToList() ?? [];

            // Get item prices
            var itemPrices = await GetItemPricesAsync(pricingItems);

            // Calculate total
            var totalEstimate = CalculateTotal(itemPrices);

            return new PriceEstimateResponse
            {
                TotalPrice = totalEstimate.Total.Amount,
                Currency = totalEstimate.Total.Currency,
                ItemBreakdown = itemPrices.Select(pr => 
                    new PriceItemResponse
                    {
                        ServiceId = pr.ServiceId,
                        ItemPrice = pr.Price.Amount,
                        Currency = pr.Price.Currency
                    }
                ).ToList()
            };
        }
    }
}
