using Deliveryix.Commons.Application.Messaging;
using Deliveryix.Commons.Domain.Results;
using Modules.Identity.Application.AccessManagement.Repositories;
using Modules.Identity.Domain.AcessManagement.Models;

namespace Modules.Identity.Application.AccessManagement.UseCases.GetAllRoles
{
    internal sealed class GetAllRolesQueryHandler(IRoleRepository roleRepository) : IQueryHandler<GetAllRolesQuery, PagedResult<Role>>
    {
        public async Task<Result<PagedResult<Role>>> ExecuteAsync(GetAllRolesQuery request, CancellationToken cancellationToken = default)
        {
            var roles = await roleRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            return roles;
        }
    }
}