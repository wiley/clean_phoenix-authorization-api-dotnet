using Authorization.Domain.User;
using Authorization.Domain.UserCache;
using System.Collections.Generic;

namespace Authorization.API.Responses
{
    public class UserResponse : GenericEntityResponse
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Status { get; set; }

        public List<UserMapping> UserMappings { get; set; }

        public List<OrganizationRole> OrganizationRoles { get; set; }
        public List<AccountRole> AccountRoles { get; set; }
        public List<GroupMembership> GroupMemberships { get; set; }

        public List<Entitlement> Entitlements { get; set; }
    }
}
