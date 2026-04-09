using Financii.Application.Controllers;
using Financii.Application.Interfaces.AppServices;
using Financii.Application.Interfaces.Repositories;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Repositories;
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

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositórios — convenção: tudo que implementa IRepositoryBase
            services.Scan(scan => scan
                .FromAssemblyOf<UnitOfWork>()
                .AddClasses(c => c.AssignableTo(typeof(IRepositoryBase<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AppServices — convenção: tudo que implementa IAppService
            services.Scan(scan => scan
                .FromAssemblyOf<BaseController>()
                .AddClasses(c => c.AssignableTo<IAppService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
