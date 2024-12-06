using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Authorization.Domain.UserCache
{
    [BsonIgnoreExtraElements]
    public class Account
    {
        [BsonIgnoreIfNull]
        [Required]
        public Guid Id { get; set; }
    }
}