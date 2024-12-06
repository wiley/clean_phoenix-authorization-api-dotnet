using Authorization.Domain.Group.GroupMembership;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authorization.Services.Interfaces
{
    public interface IGroupService
    {
        Task<List<GroupMembershipAPIRepresentation>> GetUserGroupMemberships(int UserId);
    }
}
