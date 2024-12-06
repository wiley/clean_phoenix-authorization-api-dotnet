namespace Authorization.Consumer.Domain
{
    public class UserModel
    {
        public int userID { get; set; }
        public string username { get; set; }
        public int status { get; set; }
        public DateTime createdAt { get; set; }
        public int createdBy { get; set; }
        public int updatedBy { get; set; }
        public object userMappings { get; set; }
    }

}
