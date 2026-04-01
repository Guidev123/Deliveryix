using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.Identities.Repositories;
using Modules.Identity.Domain.Identities.Errors;

namespace Modules.Identity.Application.Identities.Create
{
    internal sealed class CreateIdentityCommandHandler(
        //IMicrosoftGraphService graphService,
        IIdentityRepository identityRepository
        ) : ICommandHandler<CreateIdentityCommand, CreateIdentityResponse>
    {
        public async Task<Result<CreateIdentityResponse>> ExecuteAsync(CreateIdentityCommand request, CancellationToken cancellationToken = default)
        {
            var alreadyExists = await identityRepository.ExistsAsync(request.Document, cancellationToken);
            if (alreadyExists)
            {
                return Result.Failure<CreateIdentityResponse>(IdentityErrors.AlreadyExists(request.Document));
            }

            var userResult = await graphService.GetIdentityProviderUserAsync(request.Email, cancellationToken);
            if (userResult.IsFailure)
            {
                return Result.Failure<CreateIdentityResponse>(userResult.Error!);
            }

            var identity = Domain.Identities.Entities.Identity.Create(
                userResult.Value.ObjectId,
                request.Document,
                request.Email,
                request.Phone
                );

            identityRepository.Insert(identity);

            return new CreateIdentityResponse(identity.Id, identity.IdentityProviderId);
        }
    }
}