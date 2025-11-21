using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class PaymentMethodRepository(ApplicationDbContext context) : BaseRepository<PaymentMethodEntity>(context), IPaymentMethodRepository
    {
    }
}
