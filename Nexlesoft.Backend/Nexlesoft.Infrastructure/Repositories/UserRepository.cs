using Microsoft.EntityFrameworkCore;
using Nexlesoft.Application.Interfaces;
using Nexlesoft.Domain.Entities;
using Nexlesoft.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexlesoft.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(NexlesoftDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                                .Include(x => x.Tokens)
                                .FirstOrDefaultAsync(x => x.Email.Equals(email));
        }
        public async Task<bool> CheckUerExisted(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email.Equals(email));
        }
    }
}
