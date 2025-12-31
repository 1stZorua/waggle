using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Waggle.Common.Pagination.Core;
using Waggle.Common.Pagination.Models;
using Waggle.UserService.Dtos;
using Waggle.UserService.Models;

namespace Waggle.UserService.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context) => _context = context;

        public async Task<PagedResult<User>> GetUsersAsync(PaginationRequest request)
        {
            var sortFields = new (Expression<Func<User, object>> SortBy, string Name)[]
            {
                (u => u.CreatedAt, nameof(User.CreatedAt)),
                (u => u.Id, nameof(User.Id))
            };

            return await _context.Users.AsNoTracking().ToPagedAsync(sortFields, request);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetUsersByIdAsync(UserBatchRequest request)
        {
            var ids = request.Ids?.ToList() ?? [];
            if (ids.Count == 0) return [];

            return await _context.Users
                .AsNoTracking()
                .Where(u => ids.Contains(u.Id))
                .ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}