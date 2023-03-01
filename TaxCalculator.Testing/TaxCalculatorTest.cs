using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using TaxCalculator.API;
using TaxCalculator.Domain;
using TaxCalculator.Domain.Commands;
using TaxCalculator.Domain.Contracts;
using TaxCalculator.Domain.Views;
using TaxCalculator.Storage.Exceptions;

namespace TaxCalculator.Testing
{
    public class TaxCalculatorTest
    {

        private WebApplicationFactory<Program> application;
        private HttpClient client;

        public TaxCalculatorTest()
        {
            application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    //Mocking the authentication
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication(defaultScheme: "TestScheme")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "TestScheme", options => { });
                    });
                });

            client = application.CreateClient();

        }

        [Fact]
        public async Task CreateTaxRegulationTest()
        {
            //Arrange
            var countryCode = "tr";
            var taxRate = new Domain.Commands.TaxRate(90, "everything");

            //Act
            var response = await client.PostAsync($"TaxRegulation", new StringContent(JsonConvert.SerializeObject(new CreateTaxRegulation(countryCode, new List<Domain.Commands.TaxRate> { taxRate })), Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
            var taxRegulation = await GetTaxRegulationFromStorage(countryCode);
            Assert.Equal(taxRate.RateInPercentage, taxRegulation.TaxRates.First().RateInPercentage);
            Assert.Equal(taxRate.Tag, taxRegulation.TaxRates.First().Tag);
        }

        [Fact]
        public async Task UpdateTaxRegulationTest()
        {
            //Arrange
            var countryCode = "de";
            var domainTaxRate = new Domain.TaxRate(30, "transport");
            await CreateTaxRegulationInStorage(countryCode, domainTaxRate);

            var updateCommandTaxRate = new Domain.Commands.TaxRate(50, "some");

            //Act
            var response = await client.PutAsync($"TaxRegulation", new StringContent(JsonConvert.SerializeObject(new UpdateTaxRegulation(countryCode, new List<Domain.Commands.TaxRate> { updateCommandTaxRate })), Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
            var taxRegulation = await GetTaxRegulationFromStorage(countryCode);
            Assert.Equal(updateCommandTaxRate.RateInPercentage, taxRegulation.TaxRates.First().RateInPercentage);
            Assert.Equal(updateCommandTaxRate.Tag, taxRegulation.TaxRates.First().Tag);
        }

        [Fact]
        public async Task DeleteTaxRegulationTest()
        {
            //Arrange
            var countryCode = "us";
            var domainTaxRate = new Domain.TaxRate(5, "gasoline");
            await CreateTaxRegulationInStorage(countryCode, domainTaxRate);

            //Act
            var response = await client.DeleteAsync($"TaxRegulation/{countryCode}");

            //Assert
            response.EnsureSuccessStatusCode();
            await Assert.ThrowsAsync<EntityNotFoundException>((async () => await GetTaxRegulationFromStorage(countryCode)));
        }

        [Theory]
        [InlineData("ab", 20, 100, 0, 0)]
        [InlineData("ac", 20, 0, 120, 0)]
        [InlineData("ad", 20, 0, 0, 20)]
        public async Task CalculateTaxRegulationTest(string countryCode, double vatRate, double net, double gross, double vat)
        {
            //Arrange
            await CreateTaxRegulationInStorage(countryCode, new Domain.TaxRate(vatRate, "random metadata"));
            var purchaseDto = new Purchase(countryCode, vatRate, new Amount(net, gross, vat));

            //Act
            var response = await client.PostAsync($"TaxRegulation/calculate", new StringContent(JsonConvert.SerializeObject(purchaseDto), Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var calculatedAmount = JsonConvert.DeserializeObject<CalculatedAmount>(jsonResponse);

            var mediator = application.Services.CreateScope().ServiceProvider.GetRequiredService<IMediator>();
            var domainResponse = await mediator.Send(purchaseDto);

            Assert.Equal(domainResponse.Vat, calculatedAmount.Vat);
            Assert.Equal(domainResponse.Gross, calculatedAmount.Gross);
            Assert.Equal(domainResponse.Net, calculatedAmount.Net);
        }

        [Theory]
        [InlineData("ae", 20, 0, 0, 0)]
        [InlineData("af", 20, 0, 120, 100)]
        [InlineData("ag", 20, 100, 0, 20)]
        [InlineData("ah", 20, 100, 20, 0)]
        [InlineData("ai", 20, 50, 50, 20)]
        [InlineData("aj", 20, 0, -10, 0)]
        [InlineData("ak", 20, -10, 0, 0)]
        [InlineData("al", 20, 0, 0, -10)]
        public async Task Fail_WhenWrongAmountInput_CalculateTaxRegulationTest(string countryCode, double vatRate, double net, double gross, double vat)
        {
            //Arrange
            await CreateTaxRegulationInStorage(countryCode, new Domain.TaxRate(vatRate, "random metadata"));
            var purchaseDto = new Purchase(countryCode, vatRate, new Amount(net, gross, vat));

            //Act
            var response = await client.PostAsync($"TaxRegulation/calculate", new StringContent(JsonConvert.SerializeObject(purchaseDto), Encoding.UTF8, "application/json"));

            //Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Fail_WhenWrongVatRateInput_CalculateTaxRegulationTest()
        {
            //Arrange
            var countryCode = "am";
            await CreateTaxRegulationInStorage(countryCode, new Domain.TaxRate(20, "random metadata"));
            var purchaseDto = new Purchase(countryCode, 21, new Amount(100, 0, 0));

            //Act
            var response = await client.PostAsync($"TaxRegulation/calculate", new StringContent(JsonConvert.SerializeObject(purchaseDto), Encoding.UTF8, "application/json"));
            
            //Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        private async Task CreateTaxRegulationInStorage(string countryCode, params Domain.TaxRate[] taxRates)
        {
            var unitOfWork = application.Services.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();
            unitOfWork.TaxRegulationRepository.Create(new TaxRegulation(countryCode, taxRates.ToList()));
            await unitOfWork.SaveChangesAsync();
        }

        private async Task<TaxRegulation> GetTaxRegulationFromStorage(string countryCode)
        {
            var queries = application.Services.CreateScope().ServiceProvider.GetRequiredService<ITaxRegulationQueries>();
            return await queries.GetTaxRegulation(countryCode);
        }

        public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
                : base(options, logger, encoder, clock)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                //Add necessary role claims
                var claims = new[] { new Claim(ClaimTypes.Role, "Support"), new Claim(ClaimTypes.Role, "ApiUser") };
                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "TestScheme");

                var result = AuthenticateResult.Success(ticket);

                return Task.FromResult(result);
            }
        }
    }
}