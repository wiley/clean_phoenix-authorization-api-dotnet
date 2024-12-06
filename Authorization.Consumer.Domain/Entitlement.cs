namespace Authorization.Consumer.Domain
{
    public class Entitlement
    {
        public Guid ProductId { get; set; }
        public string Type { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public ExternalReference Owner { get; set; }
        public ExternalReference Consumer { get; set; }
        public Guid Id { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ExternalReference
    {
        public Guid AccountId { get; set; }
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
    }

}
