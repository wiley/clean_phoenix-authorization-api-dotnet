using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Authorization.Domain.UserCache
{
    [BsonIgnoreExtraElements]
    public class Group
    {
        [BsonIgnoreIfNull]
        [Required]
        public int Id { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Name { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Context { get; set; }
    }
}