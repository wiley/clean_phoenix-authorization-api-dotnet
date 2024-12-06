using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Authorization.Domain.UserCache
{
    [BsonIgnoreExtraElements]
    public class AccountRole
    {
        [BsonIgnoreIfNull]
        public Account Account { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Role { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Status { get; set; }
    }
}