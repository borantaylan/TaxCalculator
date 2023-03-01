namespace TaxCalculator.Domain.Contracts
{
    /// <summary>
    /// Queries w.r.t tax regulations
    /// </summary>
    public interface ITaxRegulationQueries
    {
        /// <summary>
        /// Fetches the tax regulation by given country code.
        /// </summary>
        /// <param name="countryCode">Two letter country code.</param>
        /// <returns>Single tax regulation.</returns>
        Task<TaxRegulation> GetTaxRegulation(string countryCode);
    }
}
