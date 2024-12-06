using Groups.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Authorization.Consumer.Domain
{
    public class GroupBaseInfoV4
    {
        [Required]
        public int OrganizationID { get; set; } = 0;

        public int PrimaryCategory { get; set; } = 0;
        public int SecondaryCategory { get; set; } = 0;

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public GroupTypeEnumV4 Type { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Description { get; set; } = "";

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public ContextEnum Context { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public GroupVisibilityEnum Visibility { get; set; }
    }
}
