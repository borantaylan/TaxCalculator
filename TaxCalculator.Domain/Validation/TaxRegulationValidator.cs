using FluentValidation;

namespace TaxCalculator.Domain.Validation
{
    /// <summary>
    /// Validation logic for tax regulation.
    /// </summary>
    internal class TaxRegulationValidator : AbstractValidator<TaxRegulation>
    {
        public TaxRegulationValidator()
        {
            RuleFor(x => x.CountryCode).NotEmpty().WithMessage("Country code can not be empty.");
            RuleFor(x => x.CountryCode).Length(2).WithMessage("The country code should conform ISO 3166-1 Alpha-2 code standard.");

            RuleFor(x => x.TaxRates).NotEmpty().WithMessage("At least one tax rate is necessary.");
            RuleForEach(x => x.TaxRates).SetValidator(new TaxRateValidator());
        }
    }

    /// <summary>
    /// Validation logic for tax rate.
    /// </summary>
    internal class TaxRateValidator : AbstractValidator<TaxRate>
    {
        public TaxRateValidator()
        {
            RuleFor(x => x.RateInPercentage).NotEmpty().GreaterThan(0).LessThan(100).WithMessage("The percentage rate should be positive value and less than 100.");
        }
    }
}
