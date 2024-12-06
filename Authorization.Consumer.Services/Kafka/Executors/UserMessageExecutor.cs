using Authorization.Consumer.Domain;
using Authorization.Domain.User;
using Authorization.Infrastructure.Interface.Mongo;
using Microsoft.Extensions.Logging;
using WLS.KafkaProcessor.Services.Executors.Interfaces;

namespace Authorization.Consumer.Services.Kafka.Executors
{
    public class UserMessageExecutor : IKafkaExecutor<UserModel>
    {
        private readonly IMongoRepository<User> _mongoRepository;
        private readonly ILogger<UserMessageExecutor> _logger;
        public UserMessageExecutor(IMongoRepository<User> mongoRepository, ILogger<UserMessageExecutor> logger)
        {
            _mongoRepository = mongoRepository;
            _logger = logger;
        }
        public async Task<bool> Execute(UserModel message, string subject)
        {
            try
            {
                User user = _mongoRepository.AsQueryable().FirstOrDefault(x => x.UserId == message.userID);
                if (user == null)
                {
                    var newUser = new User
                    {
                        UserId = message.userID,
                        Username = message.username,
                        Status = message.status.ToString(),
                        CreatedBy = message.createdBy,
                        CreatedAt = message.createdAt,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _mongoRepository.InsertOneAsync(newUser);
                    return true;
                }
                user.Username = message.username;
                user.Status = message.status.ToString();
                user.UpdatedAt = DateTime.UtcNow;
                await _mongoRepository.ReplaceOneAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error to save UserId: {0}, Subject: {1}, Message: {2}", message.userID, subject, ex.Message);
                return false;
            }
        }
    }
}
