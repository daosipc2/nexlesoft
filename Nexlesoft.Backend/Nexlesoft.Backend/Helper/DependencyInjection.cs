using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nexlesoft.Application.Interfaces;
using Nexlesoft.Backend.Services;
using Nexlesoft.Backend.Services.Interfaces;
using Nexlesoft.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexlesoft.Backend.Helper
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITokenRepository, TokenRepository>();           
          
        }
    }
}
