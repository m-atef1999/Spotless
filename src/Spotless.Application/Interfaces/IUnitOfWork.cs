namespace Spotless.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Customers { get; }
        IOrderRepository Orders { get; }
        IAdminRepository Admins { get; }
        IDriverRepository Drivers { get; }
        IOrderDriverApplicationRepository OrderDriverApplications { get; }

        IPaymentRepository Payments { get; }
        IServiceRepository Services { get; }
        IReviewRepository Reviews { get; }
        ICategoryRepository Categories { get; }

        Task<int> CommitAsync();
        Task RollbackAsync();
    }
}
