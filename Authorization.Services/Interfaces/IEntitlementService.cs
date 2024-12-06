using Authorization.Domain.Entitlements;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authorization.Services.Interfaces
{
    public interface IEntitlementService
    {
        Task<List<EntitlementAPIRepresentation>> GetUserEntitlements(int userId, List<Guid> userAccounts = null, List<int> userOrganizations = null);
    }
}
