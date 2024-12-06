using Authorization.Domain.User;
using System.Threading.Tasks;

namespace Authorization.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RequestHealthCheck();
        Task<UserAPIRepresentation> GetUser(int id);
    }
}
