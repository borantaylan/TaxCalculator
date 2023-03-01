using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Domain.Commands;
using TaxCalculator.Domain.Contracts;

namespace TaxCalculator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaxRegulationController : ControllerBase
    {
        private readonly ILogger<TaxRegulationController> logger;
        private readonly ITaxRegulationQueries taxRegulationQueries;
        private readonly ITaxRegulationRepository taxRegulationRepository;
        private readonly IMediator mediator;

        public TaxRegulationController(
            ILogger<TaxRegulationController> logger,
            ITaxRegulationQueries taxRegulationQueries,
            ITaxRegulationRepository taxRegulationRepository,
            IMediator mediator)
        {
            this.logger = logger;
            this.taxRegulationQueries = taxRegulationQueries;
            this.taxRegulationRepository = taxRegulationRepository;
            this.mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Support")]
        public async Task<IActionResult> Post(CreateTaxRegulation createTaxRegulation)
        {
            await mediator.Send(createTaxRegulation);
            return Ok();
        }

        [HttpPut()]
        [Authorize(Roles = "Support")]
        public async Task<IActionResult> Put(UpdateTaxRegulation updateTaxRegulation)
        {
            await mediator.Send(updateTaxRegulation);
            return Ok();
        }

        [HttpDelete("{countryCode}")]
        [Authorize(Roles = "Support")]
        public async Task<IActionResult> Delete([FromRoute] string countryCode)
        {
            await mediator.Send(new DeleteTaxRegulation(countryCode));
            return Ok();
        }

        [HttpGet("{countryCode}")]
        [Authorize(Roles = "Support")]
        public async Task<IActionResult> Get([FromRoute] string countryCode)
        {
            return Ok(await taxRegulationQueries.GetTaxRegulation(countryCode));
        }

        [HttpPost("calculate")]
        [Authorize(Roles = "ApiUser")]
        public async Task<IActionResult> Calculate([FromBody] Purchase purchase)
        {
            var result = await mediator.Send(purchase);
            return Ok(result);
        }
    }
}