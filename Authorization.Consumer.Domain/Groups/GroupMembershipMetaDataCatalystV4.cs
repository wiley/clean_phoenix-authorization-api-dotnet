using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Authorization.Consumer.Domain
{
    public class GroupMembershipMetaDataCatalystV4
    {
        [Required]
        public bool IsHidden { get; set; }

        [Required]
        public bool IsFavorite { get; set; }
    }
}
