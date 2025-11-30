using Spotless.Domain.Entities;

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
        INotificationRepository Notifications { get; }
        IAuditLogRepository AuditLogs { get; }
        ISystemSettingRepository SystemSettings { get; }
        IPaymentMethodRepository PaymentMethods { get; }
        IRepository<DriverApplication> DriverApplications { get; }

        Task<int> CommitAsync();
        Task RollbackAsync();
    }
}
