using Microsoft.EntityFrameworkCore;
using TaxCalculator.Domain;
using TaxCalculator.Domain.Contracts;
using TaxCalculator.Storage.Exceptions;

namespace TaxCalculator.Storage
{
    internal class TaxRegulationRepository : ITaxRegulationRepository, ITaxRegulationQueries
    {
        private readonly TaxRegulationContext taxRegulationContext;

        public TaxRegulationRepository(TaxRegulationContext taxRegulationContext)
        {
            this.taxRegulationContext = taxRegulationContext;
        }

        #region Commands
        /// <inheritdoc/>
        public void Create(TaxRegulation taxRegulation)
        {
            taxRegulationContext.TaxRegulations.Add(taxRegulation);
        }

        /// <inheritdoc/>
        public async Task Delete(string countryCode)
        {
            var taxRegulation = await taxRegulationContext.TaxRegulations.SingleOrDefaultAsync(x => x.CountryCode == countryCode);
            if (taxRegulation == null)
            {
                throw new EntityNotFoundException();
            }
            taxRegulationContext.TaxRegulations.Remove(taxRegulation);
        }

        /// <inheritdoc/>
        public async Task<TaxRegulation> Find(string countryCode)
        {
            var taxRegulation = await taxRegulationContext.TaxRegulations.SingleOrDefaultAsync(x => x.CountryCode == countryCode);
            if (taxRegulation == null)
            {
                throw new EntityNotFoundException();
            }
            return taxRegulation;
        }

        /// <inheritdoc/>
        public void Update(TaxRegulation taxRegulation)
        {
            //No need for operation since we use tracked version for now.
        }

        #endregion

        #region Queries

        /// <inheritdoc/>
        public async Task<TaxRegulation> GetTaxRegulation(string countryCode)
        {
            var taxRegulation = await taxRegulationContext.TaxRegulations.AsNoTracking().SingleOrDefaultAsync(x => x.CountryCode == countryCode);
            if (taxRegulation == null)
            {
                throw new EntityNotFoundException();
            }
            return taxRegulation;
        }

        #endregion
    }
}