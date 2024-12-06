using Authorization.Domain.UserCache;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Authorization.Domain.User
{
    [BsonCollection("user")]
    [BsonIgnoreExtraElements]
    public class User: GenericEntity
    {
        [BsonIgnoreIfNull]
        [Required]
        public int UserId { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Username { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public string Status { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public List<UserMapping> UserMappings { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public List<AccountRole> AccountRoles { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public List<OrganizationRole> OrganizationRoles { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public List<Entitlement> Entitlements { get; set; }

        [BsonIgnoreIfNull]
        [Required]
        public List<GroupMembership> GroupMemberships { get; set;}

        public bool ShouldSerializeUserMappings()
        {
            return UserMappings?.Count > 0;
        }

        public bool ShouldSerializeAccountRoles()
        {
            return AccountRoles?.Count > 0;
        }

        public bool ShouldSerializeOrganizationRoles()
        {
            return OrganizationRoles?.Count > 0;
        }

        public bool ShouldSerializeEntitlements()
        {
            return Entitlements?.Count > 0;
        }

        
    }
}