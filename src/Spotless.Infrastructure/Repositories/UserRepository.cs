using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Infrastructure.Identity;

namespace Spotless.Infrastructure.Repositories
{
    public class UserRepository(UserManager<ApplicationUser> userManager) : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<IAuthUser?> GetUserByIdAsync(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<IEnumerable<IAuthUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }
    }
}
