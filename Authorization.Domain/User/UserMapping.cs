using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Authorization.Domain.User
{
    [BsonIgnoreExtraElements]
    public class UserMapping
    {
        [BsonIgnoreIfNull]
        [Required]
        public int Id { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string PlatformName { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string platformCustomer { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string PlatformRole { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string PlatformUserId { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string PlatformAccountId { get; set; }

    }
}