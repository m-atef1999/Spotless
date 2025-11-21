using MediatR;
using Spotless.Application.Dtos.PaymentMethods;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using System.Collections.Generic;

namespace Spotless.Application.Features.Customers.Queries.GetPaymentMethods
{
    public class GetPaymentMethodsQueryHandler : IRequestHandler<GetPaymentMethodsQuery, List<PaymentMethodDto>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public GetPaymentMethodsQueryHandler(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<List<PaymentMethodDto>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
        {
            var paymentMethods = await _paymentMethodRepository.GetAsync(x => x.CustomerId == request.CustomerId);
            
            return paymentMethods.Select(pm => new PaymentMethodDto
            {
                Id = pm.Id,
                Type = pm.Type,
                Last4Digits = pm.Last4Digits,
                CardholderName = pm.CardholderName,
                ExpiryDate = pm.ExpiryDate,
                IsDefault = pm.IsDefault
            }).ToList();
        }
    }
}
