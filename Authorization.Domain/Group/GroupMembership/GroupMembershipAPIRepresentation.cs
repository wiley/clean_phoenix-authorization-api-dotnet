namespace Authorization.Domain.Group.GroupMembership
{
    public class GroupMembershipAPIRepresentation
    {
        public int GroupId { get; set; }

        public int OrganizationId { get; set; }

        public string Name { get; set; }

        public GroupMembershipDetailsAPIRepresentation Details { get; set; }
    }

    public class GroupMembershipDetailsAPIRepresentation
    {
        public int Type { get; set; }
        
        public string Context { get; set; }

        public string Status { get; set; }
        
        public string Visibility { get; set; }
    }
}
