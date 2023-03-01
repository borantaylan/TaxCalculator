using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaxCalculator.Domain.Contracts;

namespace TaxCalculator.Storage
{
    public static class DependencyInjectionExtensions
    {
        public static void AddStorage(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITaxRegulationQueries, TaxRegulationRepository>();
            services.AddScoped<ITaxRegulationRepository, TaxRegulationRepository>();
            services.AddDbContext<TaxRegulationContext>(options =>
                options.UseInMemoryDatabase("TaxRegulationDb"),
                ServiceLifetime.Scoped);
        }
    }
}
