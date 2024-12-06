using Authorization.Domain;
using Authorization.Domain.Group.GroupMembership;
using Authorization.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Authorization.Services
{
    public class GroupService : IGroupService
    {
        private readonly HttpClient _client;
        private readonly ILogger<GroupService> _logger;
        private readonly int PageSize = 20;


        public GroupService(IHttpClientFactory clientFactory, ILogger<GroupService> logger)
        {
            _client = clientFactory.CreateClient("groupService");
            _client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("GROUPS_API_BASE_URL"));
            _client.DefaultRequestHeaders.Add("GroupsAPIToken", Environment.GetEnvironmentVariable("GROUPS_API_KEY"));
            _client.Timeout = TimeSpan.FromSeconds(30);

            _logger = logger;
        }

        public async Task<List<GroupMembershipAPIRepresentation>> GetUserGroupMemberships(int UserId)
        {
            try
            {
                List<GroupMembershipAPIRepresentation> groupMemberships = new();
                string url = $"{_client.BaseAddress}api/v1/groups/search";
                var query = new Dictionary<string, string>()
                {
                    ["memberID"] = UserId.ToString(),
                    ["memberType"] = "All",
                    ["visibility"] = "All",
                    ["type"] = "1",
                    ["include"] = "details"
                };
                List<GroupMembershipAPIRepresentation> groupMembershipsLearner = await GetListResponse<GroupMembershipAPIRepresentation>(url, query);
                
                if (groupMembershipsLearner.Any()) {
                    groupMemberships.AddRange(groupMembershipsLearner);
                }

                query["type"] = "2";
                List<GroupMembershipAPIRepresentation> groupMembershipsAdmin = await GetListResponse<GroupMembershipAPIRepresentation>(url, query);

                if (groupMembershipsAdmin != null) {
                    groupMemberships.AddRange(groupMembershipsAdmin);
                }

                return groupMemberships;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GroupService (GetUserGroupMemberships) - Unhandled Exception");
                return null;
            }
        }

        private async Task<List<T>> GetListResponse<T>(string url, Dictionary<string, string> query = null) 
        {
            try
            {
                int currentOffset = PageSize;
                List<T> listItems = new();

                HttpResponseMessage response = await _client.GetAsync(QueryHelpers.AddQueryString(url, query));
                response.EnsureSuccessStatusCode();

                ListAPIV1Representation<T> responseList = await response.Content.ReadFromJsonAsync<ListAPIV1Representation<T>>();
                listItems.AddRange(responseList.Content);

                while (currentOffset < responseList._links.Count)
                {
                    response = await _client.GetAsync(responseList._links.Next.Href);
                    response.EnsureSuccessStatusCode();

                    responseList = await response.Content.ReadFromJsonAsync<ListAPIV1Representation<T>>();

                    listItems.AddRange(responseList.Content);
                    currentOffset += PageSize;
                }
                return listItems;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"GroupService (GetListResponse) - Unhandled Exception");
                return null;
            }
        }
    }
}
