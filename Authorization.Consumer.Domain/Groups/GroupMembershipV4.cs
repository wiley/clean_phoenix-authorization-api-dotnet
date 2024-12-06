using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Authorization.Consumer.Domain
{
    //Membership object, all info
    public class GroupMembershipV4 : GroupMembershipBaseV4
    {
        [Required]
        public int Id { get; set; } //????????????????????? ---- GroupMemberId

        // Note: API returns UNIX timestamps - use UnixTimeHelper to convert
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        [Required]
        public int UpdatedBy { get; set; }
        
    }
}
