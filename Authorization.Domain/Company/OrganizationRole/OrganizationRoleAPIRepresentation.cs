namespace Authorization.Domain.Company.OrganizationRole
{
    public class OrganizationRoleAPIRepresentation
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int OrganizationId { get; set; }

        public string Role { get; set; }
    }
}
