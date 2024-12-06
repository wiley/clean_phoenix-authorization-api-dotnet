using System.ComponentModel.DataAnnotations;

namespace Authorization.API.Requests
{
    public class AuthzUserCreateRequest
    {
        [Required]
        public int UserId { get; set; }
    }
}
