using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Authorization.Domain.UserCache
{
    [BsonIgnoreExtraElements]
    public class OrganizationRole
    {
        [BsonIgnoreIfNull]
        [Required]
        public Organization Organization { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Role { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Status { get; set; }
    }
}