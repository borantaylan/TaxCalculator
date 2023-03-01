namespace TaxCalculator.Domain.Contracts
{
    /// <summary>
    /// CreateTaxRegulation repo, having CRUD operation definitions.
    /// </summary>
    public interface ITaxRegulationRepository
    {
        /// <summary>
        /// Fetches the tracked tax regulation object by country code.
        /// </summary>
        /// <param name="countryCode">Two character country code.</param>
        Task<TaxRegulation> Find(string countryCode);

        /// <summary>
        /// Creates a tax regulation.
        /// </summary>
        /// <param name="taxRegulation">Tax regulation object to be added.</param>
        void Create(TaxRegulation taxRegulation);

        /// <summary>
        /// Deletes the tax regulation by country code.
        /// </summary>
        /// <param name="countryCode">Two character country code.</param>
        /// <returns></returns>
        Task Delete(string countryCode);

        /// <summary>
        /// Updates the tax regulation.
        /// </summary>
        /// <param name="taxRegulation">Tax regulation object to be updated.</param>
        void Update(TaxRegulation taxRegulation);
    }
}
