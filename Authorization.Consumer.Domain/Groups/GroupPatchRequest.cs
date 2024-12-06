using Groups.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Authorization.Consumer.Domain
{
    public class GroupPatchRequest
    {
        public int? OrganizationID { get; set; }
        public int? PrimaryCategory { get; set; }
        public int? SecondaryCategory { get; set; }
        public GroupTypeEnumV4? Type { get; set; }
        
        [StringLength(100)] //NAO BATE COM A DOC, banco limitado a 100
        public string Title { get; set; }
        
        [StringLength(255)]
        public string Description { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ContextEnum? Context { get; set; }

        public bool? IsActive { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public GroupVisibilityEnum? Visibility { get; set; }
    }
}
