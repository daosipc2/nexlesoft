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
    public class TokenRepository : Repository<Token>, ITokenRepository
    {
        public TokenRepository(NexlesoftDbContext context) : base(context)
        {
        }

        public async Task<Token> GetByUserId(int userId)
        {
            return await _context.Tokens.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<bool> DeleteAllByUserId(int userId)
        {
            try
            {
                var tokens = await _context.Tokens
                   .Where(t => t.UserId == userId)
                   .ToListAsync();

                if (tokens.Any())
                {
                    _context.Tokens.RemoveRange(tokens);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // TODO: log error
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateRefreshToken(int tokenId, string refreshToken)
        {
            var token = await _context.Tokens.FirstOrDefaultAsync(x => x.Id == tokenId);
            if (token == null)
            {
                throw new Exception("Token Id not found.");
            }

            token.UpdatedAt = DateTime.UtcNow;
            token.RefreshToken = refreshToken;
            token.ExpiresIn = DateTime.UtcNow.AddDays(30).ToString(); //new refresh token which will expires in 30 days
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Token> GetByRefreshToken(string refreshToken)
        {
            return await _context.Tokens.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        }
    }
}
