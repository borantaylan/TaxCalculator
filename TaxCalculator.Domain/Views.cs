namespace TaxCalculator.Domain.Views
{
    public record CalculatedAmount(double Net, double Gross, double Vat, double VatRate);
}
