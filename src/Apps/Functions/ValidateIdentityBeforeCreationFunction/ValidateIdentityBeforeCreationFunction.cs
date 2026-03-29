using Dapper;
using Deliveryix.Commons.Infrastructure.Factories;
using Deliveryix.Commons.Infrastructure.Serializer;
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
    private readonly string _extensionAppId;

    public ValidateIdentityBeforeCreationFunction(ILogger<ValidateIdentityBeforeCreationFunction> logger, SqlConnectionFactory sqlConnectionFactory, IConfiguration configuration)
    {
        _logger = logger;
        _sqlConnectionFactory = sqlConnectionFactory;
        _extensionAppId = configuration["Entra_ExtensionsAppId"]!;
    }

    [Function("ValidateIdentityBeforeCreation")]
    public async Task<IActionResult> Run(
     [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
     CancellationToken cancellationToken)
    {
        _logger.LogInformation("ValidateIdentityBeforeCreation triggered");

        var body = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);

        var request = JsonSerializer.Deserialize<EntraAttributeCollectionRequest>(body, JsonSerializerOptionsShared.GetDefault());

        var attributes = request?.Data?.UserSignUpInfo?.Attributes;

        if (attributes is null)
        {
            _logger.LogWarning("Attributes are returning null");
            return new OkObjectResult(Extensions.ResponseExtensions.Block("Unable to process your registration."));
        }

        var email = request?.Data?.UserSignUpInfo?.Identities?.FirstOrDefault()?.IssuerAssignedId;
        var documentNumber = attributes.GetValueOrDefault($"extension_{_extensionAppId}_DocumentNumber")?.Value;

        if (email is null || documentNumber is null)
        {
            _logger.LogWarning("Required attributes missing");
            return new OkObjectResult(Extensions.ResponseExtensions.Block("Unable to process your registration."));
        }

        var isUnique = await IsUniqueAsync(documentNumber, email, cancellationToken);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("User with email {Email} is unique: {IsUnique}", email, isUnique);
        }

        return isUnique
            ? Extensions.ResponseExtensions.Continue()
            : Extensions.ResponseExtensions.Block("An account with this email or document already exists.");
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
                            FROM [identity].Identities
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