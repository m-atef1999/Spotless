using MediatR;
using System.Collections.Generic;
using Spotless.Application.Dtos.PaymentMethods;

namespace Spotless.Application.Features.Customers.Queries.GetPaymentMethods
{
    public class GetPaymentMethodsQuery : IRequest<List<PaymentMethodDto>>
    {
        public Guid CustomerId { get; set; }

        public GetPaymentMethodsQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
