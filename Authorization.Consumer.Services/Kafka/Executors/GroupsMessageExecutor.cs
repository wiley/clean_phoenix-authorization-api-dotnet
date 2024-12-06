using Authorization.Consumer.Domain;
using Authorization.Domain.User;
using Authorization.Domain.UserCache;
using Authorization.Infrastructure.Interface.Mongo;
using Authorization.Services.Interfaces;
using Microsoft.Extensions.Logging;
using WLS.KafkaProcessor.Services.Executors.Interfaces;

namespace Authorization.Consumer.Services.Kafka.Executors
{
    public class GroupsMessageExecutor : IKafkaExecutor<GroupV4>
    {
        private readonly IMongoRepository<User> _mongoRepository;
        private readonly ILogger<GroupsMessageExecutor> _logger;
        private readonly IUserService _userService;
        public GroupsMessageExecutor(IMongoRepository<User> mongoRepository, ILogger<GroupsMessageExecutor> logger, IUserService userService) 
        {
            _mongoRepository = mongoRepository;
            _logger = logger;
            _userService = userService;
        }

        public async Task<bool> Execute(GroupV4 message, string subject)
        {
            if (message.Memberships is null || message.Memberships.Count <= 0)
            {
                return false;
            }
            foreach (var member in message.Memberships)
            {
                var user = _mongoRepository.AsQueryable().FirstOrDefault(x => x.UserId == member.UserId);
                var newMemberships = new List<GroupMembership>();
                newMemberships.Add(new GroupMembership
                {
                    Context = message.Context.ToString(),
                    GroupId = message.Id,
                    Name = message.Title,
                    Type = (int)message.Type,
                    OrganizationId = message.OrganizationID,
                    Visibility = message.Visibility.ToString(),
                });
                if (user == null)
                {
                    var newUserApi = await _userService.GetUser(member.UserId);

                    var newUser = new User
                    {
                        UserId = newUserApi.UserID,
                        Username = newUserApi.UserName,
                        Status = newUserApi.Status.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        GroupMemberships = newMemberships
                    };

                    await _mongoRepository.InsertOneAsync(newUser);
                    return true;
                }
                user.GroupMemberships = newMemberships;
                await _mongoRepository.ReplaceOneAsync(user);
                return true;
            }
            return true;
        }
    }
}
