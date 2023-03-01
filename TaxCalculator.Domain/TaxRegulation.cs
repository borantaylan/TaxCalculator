namespace TaxCalculator.Domain
{
    /// <summary>
    /// The tax information consists of a country code and variety number of rates.
    /// </summary>
    public class TaxRegulation
    {
        private TaxRegulation() { }

        /// <summary>
        /// Constructor having country code and a list of tax rates
        /// </summary>
        /// <param name="countryCode">The ISO 3166-1 Alpha-2 code.</param>
        /// <param name="taxRates">The list of different tax rates varying based on countries and goods.</param>
        public TaxRegulation(string countryCode, ICollection<TaxRate> taxRates)
        {
            CountryCode = countryCode;
            TaxRates = taxRates;
        }

        /// <summary>
        /// The ISO 3166-1 Alpha-2 code.
        /// </summary>
        public string CountryCode { get; }

        /// <summary>
        /// The list of different tax rates varying based on countries and goods.
        /// </summary>
        public ICollection<TaxRate> TaxRates { get; internal set; }
    }
}