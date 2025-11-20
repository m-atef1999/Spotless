using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<decimal> GetWalletBalanceAsync(Guid customerId);

        Task<Customer?> GetByEmailAsync(string email);
    }
}
