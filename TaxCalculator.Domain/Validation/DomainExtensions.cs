using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaxCalculator.Domain.Commands;

namespace TaxCalculator.Domain.Validation
{
    /// <summary>
    /// Extension for service collection
    /// </summary>
    public static class DomainExtensions
    {
        /// <summary>
        /// Adds domain validation to service collection
        /// </summary>
        /// <param name="serviceCollection">DI service collection</param>
        public static void AddDomainValidation(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IValidator<TaxRegulation>, TaxRegulationValidator>();
            serviceCollection.AddScoped<IValidator<TaxRate>, TaxRateValidator>();
            serviceCollection.AddScoped<IValidator<Purchase>, PurchaseValidator>();
            serviceCollection.AddScoped<IValidator<Amount>, AmountValidator>();
        }
    }
}
