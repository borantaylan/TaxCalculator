using MediatR;
using TaxCalculator.Domain.Views;

namespace TaxCalculator.Domain.Commands
{
    public record Purchase(string CountryCode, double VatRate, Amount Amount) : IRequest<CalculatedAmount>; 
    public record Amount(double Net, double Gross, double Vat);

    public record CreateTaxRegulation(string CountryCode, IEnumerable<TaxRate> TaxRates) : IRequest;
    public record UpdateTaxRegulation(string CountryCode, IEnumerable<TaxRate> TaxRates) : IRequest;
    public record DeleteTaxRegulation(string CountryCode) : IRequest;

    public record TaxRate(double RateInPercentage, string Tag);

}
