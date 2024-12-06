using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Authorization.Domain.UserCache
{
    [BsonIgnoreExtraElements]
    public class Organization
    {
        [BsonIgnoreIfNull]
        [Required]
        public int Id { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string OrganizationName { get; set; }
    }
}