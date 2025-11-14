using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<decimal> GetWalletBalanceAsync(Guid customerId)
        {
            return await _dbContext.Customers
                .Where(c => c.Id == customerId)
                .Select(c => c.WalletBalance.Amount)
                .FirstOrDefaultAsync();
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}
