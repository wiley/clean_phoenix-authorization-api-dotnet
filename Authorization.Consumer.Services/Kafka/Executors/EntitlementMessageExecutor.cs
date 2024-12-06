using Authorization.Domain.User;
using Authorization.Domain.UserCache;
using Authorization.Infrastructure.Interface.Mongo;
using Authorization.Services.Interfaces;
using Microsoft.Extensions.Logging;
using WLS.KafkaProcessor.Services.Executors.Interfaces;

namespace Authorization.Consumer.Services.Kafka.Executors
{
    public class EntitlementMessageExecutor : IKafkaExecutor<Domain.Entitlement>
    {
        private readonly IMongoRepository<User> _mongoRepository;
        private readonly ILogger<EntitlementMessageExecutor> _logger;
        private readonly IEntitlementService _entitlementService;
        private readonly IUserService _userService;
        public EntitlementMessageExecutor(IMongoRepository<User> mongoRepository, ILogger<EntitlementMessageExecutor> logger, IEntitlementService entitlementService, IUserService userService)
        {
            _mongoRepository = mongoRepository;
            _logger = logger;
            _entitlementService = entitlementService;
            _userService = userService;
        }
        public async Task<bool> Execute(Domain.Entitlement message, string subject)
        {
            try
            {
                var users = _mongoRepository.AsQueryable().Where(x => x.UserId == message.Owner.UserId || x.UserId == message.Consumer.UserId);
                if (users == null)
                {
                    var userOwner = await _userService.GetUser(message.Owner.UserId);
                    var userConsumer = await _userService.GetUser(message.Consumer.UserId);

                    var owner = new Authorization.Domain.UserCache.ExternalReference
                    {
                        AccountId = message.Owner.AccountId,
                        OrganizationId = message.Owner.OrganizationId,
                        UserId = message.Owner.UserId
                    };
                    var consumer = new Authorization.Domain.UserCache.ExternalReference
                    {
                        AccountId = message.Consumer.AccountId,
                        OrganizationId = message.Consumer.OrganizationId,
                        UserId = message.Consumer.UserId
                    };
                    var newEntitlements = new List<Authorization.Domain.UserCache.Entitlement>
                        {
                            new Authorization.Domain.UserCache.Entitlement
                            {
                                Id = message.Id,
                                ProductId = message.ProductId,
                                Type = message.Type,
                                Reference = message.Reference,
                                Status = message.Status,                                
                                Owner = owner,
                                Consumer = consumer,
                            }
                        };
                    if (userOwner is not null)
                    {
                        var newOwnerUser = new User
                        {
                            UserId = userOwner.UserID,
                            Username = userOwner.UserName,
                            Status = userOwner.Status,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            AccountRoles = new List<AccountRole>(),
                            Entitlements = newEntitlements
                        };
                        await _mongoRepository.InsertOneAsync(newOwnerUser);
                    }

                    if (userConsumer is not null)
                    {
                        var newOwnerUser = new User
                        {
                            UserId = userConsumer.UserID,
                            Username = userConsumer.UserName,
                            Status = userConsumer.Status,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            AccountRoles = new List<AccountRole>(),
                            Entitlements = newEntitlements
                        };
                        await _mongoRepository.InsertOneAsync(newOwnerUser);
                    }

                    return true;
                }
                foreach (var user in users)
                {
                    var updateEntitlements = new List<Authorization.Domain.UserCache.Entitlement>
                    {
                        new Authorization.Domain.UserCache.Entitlement
                        {
                            Id = message.Id,
                            ProductId = message.ProductId,
                            Type = message.Type,
                            Reference = message.Reference,
                            Status = message.Status,
                            Owner = new Authorization.Domain.UserCache.ExternalReference
                            {
                                AccountId = message.Owner.AccountId,
                                OrganizationId = message.Owner.OrganizationId,
                                UserId = message.Owner.UserId
                            },
                            Consumer = new Authorization.Domain.UserCache.ExternalReference
                            {
                                AccountId = message.Consumer.AccountId,
                                OrganizationId = message.Consumer.OrganizationId,
                                UserId = message.Consumer.UserId
                            },
                        }
                    };

                    user.UpdatedAt = DateTime.UtcNow;
                    user.Entitlements = updateEntitlements;

                    await _mongoRepository.ReplaceOneAsync(user);
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error to save Entitlement: {0}, Subject: {1}, Message: {2}", message.Id, subject, ex.Message);
                return false;
            }
        }
    }
}
