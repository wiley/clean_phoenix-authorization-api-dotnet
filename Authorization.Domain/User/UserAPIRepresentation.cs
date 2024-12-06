namespace Authorization.Domain.User
{
    public class UserAPIRepresentation
    {
        public int Id { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Status { get; set; }
    }
}
