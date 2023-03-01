## Task
User story
As an API user, I would like to calculate Net, Gross, VAT amounts for purchases in Austria so that I can use correctly calculated purchase data.

ACCEPTANCE CRITERIA
If the API receives one of the net, gross or VAT amounts and additionally a valid Austrian VAT rate (10%, 13%, 20%), the other two missing amounts (net/gross/VAT) are calculated by the system and returned to the client in a meaningful structure

The API provides an error with meaningful error messages, in case of:
missing or invalid (0 or non-numeric) amount input
more than one input
missing or invalid (0 or non-numeric) VAT rate input

TECHNICAL REQUIREMENTS
the solution needs to be implemented in .NET Core or later (.NET 6) 
the solution needs to use dependency injection (DI) software design pattern
the API must fulfil the REST API standards
the application needs to use Nuget package manager

## Discussion
- CQRS is applied. Mediatr is used.
- EFCore EntityFramework is used with InMemory option.
- Unit of work pattern is applied.
- Authentication and authorization are setup with a symmetric key(testing purposes). An endpoint created to generate signed tokens.
- REST applied.
- Fluent validation is used.
- XUnit is used for testing.
