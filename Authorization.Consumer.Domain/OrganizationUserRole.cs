namespace Authorization.Consumer.Domain
{
    public class OrganizationUserRole
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
        public int OrganizationRoleId { get; set; }
    }
}
