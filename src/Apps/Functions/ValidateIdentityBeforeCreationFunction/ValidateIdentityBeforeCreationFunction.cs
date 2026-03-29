using Dapper;
using Deliveryix.Commons.Infrastructure.Factories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ValidateIdentityBeforeCreationFunction.Models;

namespace ValidateIdentityBeforeCreationFunction;

public class ValidateIdentityBeforeCreationFunction
{
    private readonly ILogger<ValidateIdentityBeforeCreationFunction> _logger;
    private readonly SqlConnectionFactory _sqlConnectionFactory;
    private readonly string _extensionPrefix;

    public ValidateIdentityBeforeCreationFunction(ILogger<ValidateIdentityBeforeCreationFunction> logger, SqlConnectionFactory sqlConnectionFactory, IConfiguration configuration)
    {
        _logger = logger;
        _sqlConnectionFactory = sqlConnectionFactory;
        _extensionPrefix = $"extension_{configuration["Entra:ExtensionsAppId"]}_";
    }

    [Function("ValidateIdentityBeforeCreation")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ValidateIdentityBeforeCreation triggered");

        var body = await JsonSerializer.DeserializeAsync<JsonElement>(
            req.BodyReader, cancellationToken: cancellationToken);

        var email = body.GetString("email");
        var documentNumber = body.GetString($"{_extensionPrefix}DocumentNumber");

        if (email is null || documentNumber is null)
        {
            _logger.LogWarning("Required claims missing from EntraApiConnectorRequest");
            return new OkObjectResult(new BlockingResponse("We were unable to complete your registration. Please try again or contact support."));
        }

        var isUnique = await IsUniqueAsync(documentNumber, email, cancellationToken);

        if (!isUnique)
        {
            return new OkObjectResult(new BlockingResponse("We were unable to complete your registration. Please try again or contact support"));
        }

        return new OkObjectResult(new ContinuationResponse());
    }

    private async Task<bool> IsUniqueAsync(
        string document,
        string email,
        CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.Create();

        const string sql = """"
            SELECT
                CAST(
                    CASE
                        WHEN EXISTS(
                            SELECT 1
                            FROM identity.Identities
                            WHERE Document = @Document
                               OR Email = @Email
                        )
                        THEN 0
                        ELSE 1
                    END AS BIT
                ) AS ExistsUser;
            """";

        return await connection.ExecuteScalarAsync<bool>(sql, new { Document = document, Email = email }).WaitAsync(cancellationToken);
    }
}