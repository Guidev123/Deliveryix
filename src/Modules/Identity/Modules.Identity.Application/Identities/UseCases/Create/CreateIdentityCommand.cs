using Deliveryix.Commons.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Identity.Application.Identities.UseCases.Create
{
    public sealed record CreateIdentityCommand(string Email, string Document, string Phone) : ICommand<CreateIdentityResponse>;
}