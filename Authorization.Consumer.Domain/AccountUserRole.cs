namespace Authorization.Consumer.Domain
{
    public class AccountUserRole
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public Guid AccountId { get; set; }
        public int Role { get; set; }
    }
}
