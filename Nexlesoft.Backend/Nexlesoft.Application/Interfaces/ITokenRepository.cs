using Nexlesoft.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexlesoft.Application.Interfaces
{
    public interface ITokenRepository : IRepository<Token>
    {
        Task<Token> GetByUserId(int userId);
        Task<bool> UpdateRefreshToken(int tokenId, string refreshToken);
        Task<bool> DeleteAllByUserId(int userId);
        Task<Token> GetByRefreshToken(string refreshToken);
    }
}
