using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Waggle.Common.Pagination.Core;
using Waggle.Common.Pagination.Models;
using Waggle.UserService.Models;

namespace Waggle.UserService.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context) => _context = context;

        public async Task<PagedResult<User>> GetAllUsers(PaginationRequest request)
        {
            var sortFields = new (Expression<Func<User, object>> SortBy, string Name)[]
            {
                (u => u.CreatedAt, nameof(User.CreatedAt)),
                (u => u.Id, nameof(User.Id))
            };

            return await _context.Users.AsNoTracking().ToPagedAsync(sortFields, request);
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
