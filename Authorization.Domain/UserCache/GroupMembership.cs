using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Authorization.Domain.UserCache
{
    [BsonIgnoreExtraElements]
    public class GroupMembership
    {
        [BsonIgnoreIfNull]
        [Required]
        public int GroupId { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public int OrganizationId { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Name { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public int Type { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Context { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Visibility { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Status { get; set; }
    }
}