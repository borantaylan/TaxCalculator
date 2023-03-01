using Microsoft.Extensions.DependencyInjection;
using TaxCalculator.Domain.Contracts;

namespace TaxCalculator.Storage
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly TaxRegulationContext context;
        private readonly ITaxRegulationRepository taxRegulationRepository;

        public UnitOfWork(TaxRegulationContext context, ITaxRegulationRepository taxRegulationRepository)
        {
            this.context = context;
            this.taxRegulationRepository = taxRegulationRepository;
        }
        public ITaxRegulationRepository TaxRegulationRepository => taxRegulationRepository;

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
