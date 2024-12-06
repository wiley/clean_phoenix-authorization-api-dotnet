using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Authorization.Domain.Entitlements
{
    public class EntitlementAPIRepresentation
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string Type { get; set; }

        public string Reference { get; set; }

        public string Status { get; set; }

        public ExternalReferenceEntitlementAPI Owner { get; set; }

        public ExternalReferenceEntitlementAPI Consumer { get; set; }

    }

    public class ExternalReferenceEntitlementAPI
    {
        [BsonIgnoreIfNull]
        public Guid AccountId { get; set; }

        [BsonIgnoreIfNull]
        public int OrganizationId { get; set; }

        [BsonIgnoreIfNull]
        public int UserId { get; set; } = 0;
    }

}