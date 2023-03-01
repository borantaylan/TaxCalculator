using FluentValidation;
using TaxCalculator.Domain.Commands;
using TaxCalculator.Domain.Contracts;

namespace TaxCalculator.Domain.Validation
{
    /// <summary>
    /// Validation logic for purchases
    /// </summary>
    internal class PurchaseValidator : AbstractValidator<Purchase>
    {
        public PurchaseValidator(ITaxRegulationQueries queries)
        {
            RuleFor(x => x.VatRate).NotEmpty()
                .MustAsync(async (instance, vatRate, cancellationToken) =>
                {
                    var taxRegulation = await queries.GetTaxRegulation(instance.CountryCode.ToLower());
                    return taxRegulation.TaxRates.Any(x => x.RateInPercentage == vatRate);
                })
                .WithMessage("Specified vat rate is not matching with one of the rates conforming current regulation.");

            RuleFor(x => x.Amount).NotEmpty().SetValidator(new AmountValidator());
        }
    }

    /// <summary>
    /// Validation logic for amount net, gross and VAT
    /// </summary>
    internal class AmountValidator : AbstractValidator<Amount>
    {
        public AmountValidator()
        {
            var exactlyOneShouldBeGivenMessage = "It should be given exactly one of the values (Net, Gross, Vat) as input.";
            var nonNegativeMessage = "{PropertyName} value can not be negative number.";

            RuleFor(x => x.Net).Empty().When(x => x.Gross != default || x.Vat != default)
                .WithMessage(exactlyOneShouldBeGivenMessage);

            RuleFor(x => x.Net).NotEmpty().GreaterThan(0).When(x => x.Gross == default && x.Vat == default)
                .WithMessage(x=> x.Net >= 0 ?
                    exactlyOneShouldBeGivenMessage :
                    nonNegativeMessage
                );

            RuleFor(x => x.Gross).Empty().When(x => x.Net != default || x.Vat != default)
                .WithMessage(exactlyOneShouldBeGivenMessage);

            RuleFor(x => x.Gross).NotEmpty().GreaterThan(0).When(x => x.Net == default && x.Vat == default)
                .WithMessage(x => x.Gross >= 0 ?
                    exactlyOneShouldBeGivenMessage :
                    nonNegativeMessage
                );

            RuleFor(x => x.Vat).Empty().When(x => x.Gross != default || x.Net != default)
                .WithMessage(exactlyOneShouldBeGivenMessage);

            RuleFor(x => x.Vat).NotEmpty().GreaterThan(0).When(x => x.Gross == default && x.Net == default)
                .WithMessage(x => x.Vat >= 0 ?
                    exactlyOneShouldBeGivenMessage :
                    nonNegativeMessage
                );
        }
    }
}
