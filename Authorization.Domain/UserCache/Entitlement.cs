using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Authorization.Domain.UserCache
{
    [BsonIgnoreExtraElements]
    public class Entitlement
    {
        [BsonIgnoreIfNull]
        [Required]
        public Guid Id { get; set; }

        [BsonIgnoreIfNull]
        public Guid ProductId { get; set; }

        [BsonIgnoreIfNull]
        public string Type { get; set; }

        [BsonIgnoreIfNull]
        public string Reference { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Status { get; set; }

        [BsonIgnoreIfNull]
        public ExternalReference Owner { get; set; }

        [BsonIgnoreIfNull]
        public ExternalReference Consumer { get; set; }

    }

    public class ExternalReference
    {
        [BsonIgnoreIfNull]
        public Guid AccountId { get; set; }

        [BsonIgnoreIfNull]
        public int OrganizationId { get; set; }

        [BsonIgnoreIfNull]
        public int UserId { get; set; }

        public bool ShouldSerializeAccountId()
        {
            return AccountId != Guid.Empty;
        }

        public bool ShouldSerializeOrganizationId()
        {
            return OrganizationId > 0;
        }

        public bool ShouldSerializeUserId()
        {
            return UserId > 0;
        }
    }

}