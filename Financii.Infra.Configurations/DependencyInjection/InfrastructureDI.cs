using Financii.Application.Controllers;
using Financii.Application.Interfaces.AppServices;
using Financii.Application.Interfaces.Repositories;
using Financii.Domain.Entities;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Repositories;
using Financii.Infra.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Financii.Infra.Configurations.DependencyInjection
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FinanciiDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Identity — does not override the default scheme (JWT is configured in Program.cs)
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<FinanciiDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories — convention: everything implementing IRepositoryBase
            services.Scan(scan => scan
                .FromAssemblyOf<UnitOfWork>()
                .AddClasses(c => c.AssignableTo(typeof(IRepositoryBase<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // Repositories outside IRepositoryBase pattern (e.g. Identity)
            services.AddScoped<IUserRepository, UserRepository>();

            // Infrastructure services
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AppServices — convention: everything implementing IAppService
            services.Scan(scan => scan
                .FromAssemblyOf<BaseController>()
                .AddClasses(c => c.AssignableTo<IAppService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
