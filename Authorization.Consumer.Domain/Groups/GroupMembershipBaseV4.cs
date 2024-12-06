using Groups.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Authorization.Consumer.Domain
{
    //Object to Add/Update membership data
    public class GroupMembershipBaseV4
    {
        /*[Required]
        public int Id { get; set; } //?????????????????????*/

        [Required]
        public int UserId { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public GroupMemberEnum Type { get; set; }

        [Required]
        public bool Owner { get; set; }

        public Object Metadata { get; set; }
    }
}
