using Authorization.Domain.Company.OrganizationRole;
using Authorization.Domain.Entitlements;
using Authorization.Domain.User;
using Authorization.Domain.UserCache;
using Authorization.Domain.Group.GroupMembership;
using Authorization.Infrastructure.Interface.Mongo;
using Authorization.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Authorization.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMongoRepository<User> _userRepository;
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IEntitlementService _entitlementService;
        private readonly IGroupService _groupService;
        private readonly IPaginationService<User> _paginationService;
        private readonly ILogger<CacheService> _logger;

        private readonly Mapper _mapper;

        public CacheService(
            IMongoRepository<User> UserRepository,
            ILogger<CacheService> logger,
            IPaginationService<User> paginationService,
            IUserService userService,
            ICompanyService companyService,
            IEntitlementService entitlementService,
            IGroupService groupService
        )
        {
            _userRepository = UserRepository;
            _logger = logger;
            _paginationService = paginationService;
            _userService = userService;
            _companyService = companyService;
            _entitlementService = entitlementService;
            _groupService = groupService;

            _mapper = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<EntitlementAPIRepresentation, Entitlement>();
                cfg.CreateMap<ExternalReferenceEntitlementAPI, ExternalReference>();
            }));
        }

        public int TotalFound => _paginationService.TotalRecords;

        public async Task<User> GetUser(int UserId)
        {
            Expression<Func<User, bool>> filter = user => user.UserId == UserId;
            User user = _userRepository.FindOne(filter);
            if (user == null)
            {
                await CreateUser(UserId);
                user = _userRepository.FindOne(filter);
            }
            return user;
        }

        public async Task CreateUser(int UserId)
        {
            DeleteUser(UserId);

            UserAPIRepresentation userAPIRepresentation = _userService.GetUser(UserId).Result;

            if (userAPIRepresentation != null) {
                User user = new()
                {
                    UserId = userAPIRepresentation.UserID,
                    Username = userAPIRepresentation.UserName,
                    Status = userAPIRepresentation.Status,

                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                user.OrganizationRoles = await GetUserOrganizationRoles(UserId);

                await _userRepository.InsertOneAsync(user);

                _logger.LogInformation("CacheService - Inserted User - {UserId}", UserId);
            }
            else
            {
                _logger.LogInformation("CacheService - User Not Found - {UserId}", UserId);
            }
        }

        private async Task<List<OrganizationRole>> GetUserOrganizationRoles(int UserId)
        {
            List<OrganizationRoleAPIRepresentation> organizationRoleAPIRepresentation = await _companyService.GetOrganizationUserRoles(UserId);
            List<OrganizationRole> organizationRoles = new List<OrganizationRole>();

            organizationRoleAPIRepresentation?.ForEach(orgRole =>
            {
                OrganizationRole organizationRole = new()
                {
                    Organization = new()
                    {
                        Id = orgRole.OrganizationId
                    },
                    Role = orgRole.Role
                };
                organizationRoles.Add(organizationRole);
            });
            return organizationRoles;
        }

        private async Task<List<AccountRole>> GetUserAccountRoles(int UserId)
        {
            List<AccountUserRoleAPIRepresentation> accountRolesAPI = await _companyService.GetAccountUserRoles(UserId);
            List<AccountRole> accountRoles = new();

            accountRolesAPI?.ForEach(accRole =>
            {
                AccountRole accountRole = new()
                {
                    Account = new()
                    {
                        Id = accRole.AccountId
                    },
                    Role = accRole.Role
                };
                accountRoles.Add(accountRole);
            });
            return accountRoles;
        }

        private async Task<List<Entitlement>> GetUserEntitlements(int UserId)
        {
            List<OrganizationRole> orgRoles = await GetUserOrganizationRoles(UserId);
            List<int> orgIds = new();

            orgRoles?.ForEach(orgRole =>
            {
                orgIds.Add(orgRole.Organization.Id);
            });

            List<EntitlementAPIRepresentation> entitlementsApi = await _entitlementService.GetUserEntitlements(userId: UserId, userOrganizations: orgIds);
            List<Entitlement> entitlements = _mapper.Map<List<Entitlement>>(entitlementsApi);

            return entitlements;
        }

        private async Task<List<GroupMembership>> GetUserGroupMemberships(int UserId)
        {
            List<GroupMembershipAPIRepresentation> groupMembershipsAPI = await _groupService.GetUserGroupMemberships(UserId);
            List<GroupMembership> groupMemberships = new();

            groupMembershipsAPI?.ForEach(groupMembership =>
            {
                GroupMembership membership = new()
                {
                    GroupId = groupMembership.GroupId,
                    OrganizationId = groupMembership.OrganizationId,
                    Name = groupMembership.Name,
                    Type = groupMembership.Details.Type,
                    Context = groupMembership.Details.Context,
                    Visibility = groupMembership.Details.Visibility,
                    Status = groupMembership.Details.Status
                };
                groupMemberships.Add(membership);
            });
            return groupMemberships;
        }

        public async Task<User> UpdateUser(int id, User user)
        {
            Guid user_id = (await GetUser(id)).Id;
            var update = Builders<User>.Update.Set(u => u.UserId, id);

            if (user.OrganizationRoles != null)
                update = update.Set(u => u.OrganizationRoles, user.OrganizationRoles);

            if (user.Entitlements != null)
                update = update.Set(u => u.Entitlements, user.Entitlements);

            if (user.AccountRoles != null)
                update = update.Set(u => u.AccountRoles, user.AccountRoles);

            if (user.GroupMemberships != null)
                update = update.Set(u => u.GroupMemberships, user.GroupMemberships);

            user.UpdatedAt = DateTime.UtcNow;
            update = update.Set(e => e.UpdatedAt, user.UpdatedAt);

            await _userRepository.UpdateOneAsync(user_id, update);

            _logger.LogInformation("CacheService - User Updated - {UserId}", id);

            user = await GetUser(id);
            
            return user;
        }

        public void DeleteUser(int UserId)
        {
            Expression<Func<User, bool>> filter = user => user.UserId == UserId;
            _userRepository.DeleteOne(filter);
        }
    }
}
