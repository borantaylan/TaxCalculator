using FluentValidation;
using MediatR;
using TaxCalculator.Domain.Contracts;

namespace TaxCalculator.Domain.CommandHandlers
{
    internal class DeleteTaxRegulationCommandHandler : IRequestHandler<Commands.DeleteTaxRegulation>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteTaxRegulationCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(Commands.DeleteTaxRegulation request, CancellationToken cancellationToken)
        {
            await unitOfWork.TaxRegulationRepository.Delete(request.CountryCode);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
