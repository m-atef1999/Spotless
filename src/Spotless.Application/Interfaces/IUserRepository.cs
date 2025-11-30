using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IAuthUser?> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<IAuthUser>> GetAllUsersAsync();
    }
}
