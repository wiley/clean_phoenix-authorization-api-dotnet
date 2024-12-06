using System.ComponentModel.DataAnnotations;

namespace Authorization.Consumer.Domain
{
    //Group Base date with memberships
    public class GroupBaseV4 : GroupBaseInfoV4
    {
        [MaxLength(500)]
        public List<GroupMembershipBaseV4> Memberships { get; set; }
    }
}
