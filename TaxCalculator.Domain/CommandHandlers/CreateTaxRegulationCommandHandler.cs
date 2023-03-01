using FluentValidation;
using MediatR;
using TaxCalculator.Domain.Contracts;

namespace TaxCalculator.Domain.CommandHandlers
{
    internal class CreateTaxRegulationCommandHandler : IRequestHandler<Commands.CreateTaxRegulation>
    {
        private readonly IValidator<TaxRegulation> domainValidator;
        private readonly IUnitOfWork unitOfWork;

        public CreateTaxRegulationCommandHandler(IValidator<TaxRegulation> domainValidator, IUnitOfWork unitOfWork)
        {
            this.domainValidator = domainValidator;
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(Commands.CreateTaxRegulation request, CancellationToken cancellationToken)
        {
            var taxRegulation = new TaxRegulation(request.CountryCode.ToLower(), request.TaxRates.Select(x => new TaxRate(x.RateInPercentage, x.Tag)).ToList());
            domainValidator.ValidateAndThrow(taxRegulation);
            unitOfWork.TaxRegulationRepository.Create(taxRegulation);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
