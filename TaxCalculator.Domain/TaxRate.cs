namespace TaxCalculator.Domain
{
    /// <summary>
    /// The tax rate having a rate percentage and optional tag as metadata information to the tax rate.
    /// </summary>
    /// <param name="RateInPercentage">The rate in percentage.</param>
    /// <param name="Tag">Optional metadata.</param>
    public record TaxRate(double RateInPercentage, string Tag);
}
