using Authorization.Consumer.Domain;
using Authorization.Domain.User;
using Authorization.Domain.UserCache;
using Authorization.Infrastructure.Interface.Mongo;
using Authorization.Services.Interfaces;
using Microsoft.Extensions.Logging;
using WLS.KafkaProcessor.Services.Executors.Interfaces;

namespace Authorization.Consumer.Services.Kafka.Executors
{
    public class AccountUserRoleMessageExecutor : IKafkaExecutor<AccountUserRole>
    {
        private readonly IMongoRepository<User> _mongoRepository;
        private readonly ILogger<AccountUserRoleMessageExecutor> _logger;
        private readonly IUserService _userService;
        public AccountUserRoleMessageExecutor(IMongoRepository<User> mongoRepository, ILogger<AccountUserRoleMessageExecutor> logger, IUserService userService)
        {
            _mongoRepository = mongoRepository;
            _logger = logger;
            _userService = userService;
        }

        public async Task<bool> Execute(AccountUserRole message, string subject)
        {
            try
            {
                var user = _mongoRepository.AsQueryable().FirstOrDefault(x => x.UserId == message.UserId);
                var listRoles = new List<AccountRole>
                {
                    new AccountRole
                    {
                        Account = new Account { Id = message.AccountId },
                        Role = message.Role.ToString(),
                    }
                };
                if (user == null)
                {
                    var newUserApi = await _userService.GetUser(message.UserId);

                    var newUser = new User
                    {
                        UserId = newUserApi.UserID,
                        Username = newUserApi.UserName,
                        Status = newUserApi.Status.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        AccountRoles = listRoles
                    };

                    await _mongoRepository.InsertOneAsync(newUser);
                    return true;
                }
                user.AccountRoles = listRoles;
                await _mongoRepository.ReplaceOneAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error to save AccountUserRole: {0}, Subject: {1}, Message: {2}", message.Id, subject, ex.Message);
                return false;
            }

        }
    }
}
