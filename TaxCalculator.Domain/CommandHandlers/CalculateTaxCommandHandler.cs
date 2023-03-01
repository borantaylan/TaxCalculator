using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using TaxCalculator.Domain.Commands;
using TaxCalculator.Domain.Views;

namespace TaxCalculator.Domain.CommandHandlers
{
    internal class TaxCalculator : IRequestHandler<Purchase, CalculatedAmount>
    {
        private readonly ILogger<TaxCalculator> logger;
        private readonly IValidator<Purchase> validator;

        public TaxCalculator(ILogger<TaxCalculator> logger, IValidator<Purchase> validator)
        {
            this.logger = logger;
            this.validator = validator;
        }

        public async Task<CalculatedAmount> Handle(Purchase request, CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            if (request.Amount.Net != default)
            {
                return new CalculatedAmount(
                    request.Amount.Net,
                    request.Amount.Net + request.Amount.Net * request.VatRate / 100,
                    request.Amount.Net * request.VatRate / 100,
                    request.VatRate);
            }
            else if (request.Amount.Gross != default)
            {
                return new CalculatedAmount(
                    request.Amount.Gross / (request.VatRate + 100) * 100,
                    request.Amount.Gross,
                    request.Amount.Gross / (request.VatRate + 100) * request.VatRate,
                    request.VatRate);
            }
            else if (request.Amount.Vat != default)
            {
                return new CalculatedAmount(
                    request.Amount.Vat / request.VatRate * 100,
                    request.Amount.Vat / request.VatRate * (100 + request.VatRate),
                    request.Amount.Vat,
                    request.VatRate);
            }
            else
            {
                logger.LogCritical("Operation failed, validation is bypassed for unknown reasons. Investigate.");
                throw new InvalidOperationException();
            }
        }
    }
}
