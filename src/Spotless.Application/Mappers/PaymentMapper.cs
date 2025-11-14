using Spotless.Application.Dtos.Payment;
using Spotless.Domain.Entities;

namespace Spotless.Application.Mappers
{
    public static class PaymentMapper
    {
        public static PaymentDto ToDto(this Payment payment)
        {
            return new PaymentDto(
                Id: payment.Id,
                OrderId: payment.OrderId,
                Amount: payment.Amount.Amount,
                Currency: payment.Amount.Currency,
                PaymentDate: payment.PaymentDate,
                Method: payment.PaymentMethod,
                Status: payment.Status
            );
        }
    }
}
