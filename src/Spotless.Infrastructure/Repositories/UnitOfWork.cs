using Spotless.Application.Interfaces;
using Spotless.Infrastructure.Context;


namespace Spotless.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public IOrderRepository Orders { get; }
        public ICustomerRepository Customers { get; }
        public IAdminRepository Admins { get; }
        public IDriverRepository Drivers { get; }
        public ICategoryRepository Categories { get; }
        public IServiceRepository Services { get; }
        public IPaymentRepository Payments { get; }
        public IReviewRepository Reviews { get; }




        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            Orders = new OrderRepository(_dbContext);
            Customers = new CustomerRepository(_dbContext);
            Admins = new AdminRepository(_dbContext);
            Drivers = new DriverRepository(_dbContext);
            Categories = new CategoryRepository(_dbContext);
            Services = new ServiceRepository(_dbContext);
            Payments = new PaymentRepository(_dbContext);
            Reviews = new ReviewRepository(_dbContext);

        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}