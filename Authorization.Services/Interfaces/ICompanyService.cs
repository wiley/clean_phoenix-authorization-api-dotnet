using Authorization.Domain.Company.OrganizationRole;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authorization.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<List<OrganizationRoleAPIRepresentation>> GetOrganizationUserRoles(int UserId);

        Task<List<AccountUserRoleAPIRepresentation>> GetAccountUserRoles(int UserId);
    }
}
