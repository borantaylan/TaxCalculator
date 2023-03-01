using FluentValidation;
using MediatR;
using TaxCalculator.Domain.Contracts;

namespace TaxCalculator.Domain.CommandHandlers
{
    internal class UpdateTaxRegulationCommandHandler : IRequestHandler<Commands.UpdateTaxRegulation>
    {
        private readonly IUnitOfWork unitOfWork;

        public UpdateTaxRegulationCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(Commands.UpdateTaxRegulation request, CancellationToken cancellationToken)
        {
            var taxRegulation = await unitOfWork.TaxRegulationRepository.Find(request.CountryCode.ToLower());
            taxRegulation.TaxRates = request.TaxRates.Select(x => new TaxRate(x.RateInPercentage, x.Tag)).ToList();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
