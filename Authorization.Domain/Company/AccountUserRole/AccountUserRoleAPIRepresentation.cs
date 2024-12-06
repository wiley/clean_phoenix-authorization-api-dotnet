using System;

namespace Authorization.Domain.Company.OrganizationRole
{
    public class AccountUserRoleAPIRepresentation
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public Guid AccountId { get; set; }

        public string Role { get; set; }
    }
}
